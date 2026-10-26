-- =====================================================================================
-- Integración SECAD - VMS ISS SecurOS (caso de uso 1: cámaras cercanas y video en vivo)
-- Esquema: USR_CAD (Resources.RecursoGeneral.BaseDats_Usuario)
-- Ejecutar con un usuario con privilegios sobre USR_CAD. Compatible con Oracle 11g+.
-- =====================================================================================

-- Censo local de cámaras traído del VMS (GET /api/v1/cameras).
-- Solo se guardan los datos necesarios: nunca la configuración completa de la cámara.
CREATE TABLE USR_CAD.CADVMS_CAMARA (
    SITIO_GRABA           NUMBER(5)       NOT NULL,
    ID_VMS                VARCHAR2(64)    NOT NULL,
    NOMBRE                VARCHAR2(200),
    LATITUD               NUMBER(10,7),
    LONGITUD              NUMBER(10,7),
    ESTADO                VARCHAR2(30),
    HABILITADA            CHAR(1)         DEFAULT 'S' NOT NULL,
    PTZ                   CHAR(1)         DEFAULT 'N' NOT NULL,
    ACTIVA                CHAR(1)         DEFAULT 'S' NOT NULL,
    FECHA_SINCRONIZACION  DATE,
    CONSTRAINT PK_CADVMS_CAMARA PRIMARY KEY (SITIO_GRABA, ID_VMS),
    CONSTRAINT CK_CADVMS_CAMARA_HAB CHECK (HABILITADA IN ('S','N')),
    CONSTRAINT CK_CADVMS_CAMARA_PTZ CHECK (PTZ IN ('S','N')),
    CONSTRAINT CK_CADVMS_CAMARA_ACT CHECK (ACTIVA IN ('S','N'))
);

COMMENT ON TABLE  USR_CAD.CADVMS_CAMARA IS 'Censo de cámaras del VMS ISS SecurOS sincronizado por SECAD';
COMMENT ON COLUMN USR_CAD.CADVMS_CAMARA.ID_VMS IS 'Identificador de la cámara en SecurOS';
COMMENT ON COLUMN USR_CAD.CADVMS_CAMARA.ESTADO IS 'Estado SecurOS: ARMED, DISARMED, ALARMED, DETACHED, DISARM_DETACHED';
COMMENT ON COLUMN USR_CAD.CADVMS_CAMARA.ACTIVA IS 'N cuando la cámara ya no aparece en el VMS';

-- Búsqueda por cercanía: prefiltro por recuadro de coordenadas.
CREATE INDEX USR_CAD.IX_CADVMS_CAMARA_UBIC ON USR_CAD.CADVMS_CAMARA (SITIO_GRABA, LATITUD, LONGITUD);

-- Auditoría de cada visualización: quién, qué cámara, cuándo y contra qué caso.
CREATE SEQUENCE USR_CAD.SEQ_CADVMS_VISUALIZACION START WITH 1 INCREMENT BY 1 NOCACHE;

CREATE TABLE USR_CAD.CADVMS_VISUALIZACION (
    CONSECUTIVO     NUMBER          NOT NULL,
    SITIO_GRABA     NUMBER(5)       NOT NULL,
    NUME_LLAMADA    NUMBER(15)      NOT NULL,
    ID_VMS          VARCHAR2(64)    NOT NULL,
    NOMBRE_CAMARA   VARCHAR2(200),
    USUARIO         VARCHAR2(50)    NOT NULL,
    DIRECCION_IP    VARCHAR2(45),
    MAQUINA         VARCHAR2(100),
    FECHA           DATE            DEFAULT SYSDATE NOT NULL,
    CONSTRAINT PK_CADVMS_VISUALIZACION PRIMARY KEY (CONSECUTIVO)
);

COMMENT ON TABLE USR_CAD.CADVMS_VISUALIZACION IS 'Bitácora de visualizaciones de video del VMS desde SECAD';

CREATE INDEX USR_CAD.IX_CADVMS_VISU_CASO    ON USR_CAD.CADVMS_VISUALIZACION (SITIO_GRABA, NUME_LLAMADA);
CREATE INDEX USR_CAD.IX_CADVMS_VISU_USUARIO ON USR_CAD.CADVMS_VISUALIZACION (USUARIO, FECHA);

-- Si la aplicación se conecta con un usuario distinto del dueño del esquema, otorgar:
-- GRANT SELECT, INSERT, UPDATE ON USR_CAD.CADVMS_CAMARA TO <USUARIO_APLICACION>;
-- GRANT SELECT, INSERT ON USR_CAD.CADVMS_VISUALIZACION TO <USUARIO_APLICACION>;
-- GRANT SELECT ON USR_CAD.SEQ_CADVMS_VISUALIZACION TO <USUARIO_APLICACION>;

-- Consulta de la bitácora de un caso:
-- SELECT FECHA, USUARIO, NOMBRE_CAMARA, DIRECCION_IP, MAQUINA
--   FROM USR_CAD.CADVMS_VISUALIZACION
--  WHERE SITIO_GRABA = :sitio AND NUME_LLAMADA = :llamada
--  ORDER BY FECHA;
