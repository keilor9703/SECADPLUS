using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClProcesosIntegracion
{
    public class ClWsProcesosIntegracion
    {
       /* public ServProcesoIntegracion.ClRetorno MIngDatosGrabacionIcp(ServProcesoIntegracion.ClDatosICP DatosIn) {
            ServProcesoIntegracion.ServSecadSoapClient ServIntegracion = new ServProcesoIntegracion.ServSecadSoapClient();
            ServProcesoIntegracion.ClRetorno vsalida = new ServProcesoIntegracion.ClRetorno();
            vsalida = ServIntegracion.M1DatosLlamadaICP(DatosIn);
            return vsalida;
        }
        */

        public ClGrabacionesICP.ClCadPedidosDatosICPOut MInsDatosGrabacion(ClGrabacionesICP.ClCadPedidosDatosICP DatosGrabacion)
        {
            ClGrabacionesICP.ClCadPedidosDatosICPOut vsalida = new ClGrabacionesICP.ClCadPedidosDatosICPOut();

            ServProcesoIntegracion.ServSecadSoapClient ServIntegracion = new ServProcesoIntegracion.ServSecadSoapClient();

            ServProcesoIntegracion.ClRetorno salidaWsProcInt = new ServProcesoIntegracion.ClRetorno();
            ServProcesoIntegracion.ClDatosICP entradaWsProcInt = new ServProcesoIntegracion.ClDatosICP();

            //System.Diagnostics.EventLog.WriteEntry ("secad","acd: " + DatosGrabacion.acd);
            
            entradaWsProcInt.pSitioGraba = DatosGrabacion.cadpedi_sitiograba.ToString();
            entradaWsProcInt.pNumeroSecad = DatosGrabacion.cadpedi_numellamada.ToString();
            entradaWsProcInt.pAcd =    DatosGrabacion.acd.ToString();
            entradaWsProcInt.pTaskId = DatosGrabacion.taskid.ToString();
            entradaWsProcInt.pCaller = DatosGrabacion.caller.ToString();
            entradaWsProcInt.pRecordId = DatosGrabacion.recordid.ToString();
            entradaWsProcInt.pCalled = DatosGrabacion.called.ToString();
            entradaWsProcInt.pCallId = DatosGrabacion.callid.ToString();
            entradaWsProcInt.pUsuario = DatosGrabacion.usersecad.ToString();
            entradaWsProcInt.pEquipo = DatosGrabacion.cadequipo.ToString();

            salidaWsProcInt = ServIntegracion.M1DatosLlamadaICP(entradaWsProcInt);

            vsalida.codSalida = salidaWsProcInt.codSalida.ToString();
            vsalida.desSalida = salidaWsProcInt.desSalida.ToString();
            return vsalida;
        }


        public ClPersMediosDispRadios.ClPersMediosDispRadiosOut MConsPersMediosDispRadios(ClPersMediosDispRadios.ClPersMediosDispRadiosIn DatosConsulta)
        {
            ClPersMediosDispRadios.ClPersMediosDispRadiosOut vsalida = new ClPersMediosDispRadios.ClPersMediosDispRadiosOut();

            ServProcesoIntegracion.ServSecadSoapClient ServIntegracion = new ServProcesoIntegracion.ServSecadSoapClient();
            ServProcesoIntegracion.ClPersMediosDispRadiosOut salidaWsProcInt = new ServProcesoIntegracion.ClPersMediosDispRadiosOut();


            salidaWsProcInt = ServIntegracion.M2pPersMediosDispRadios(DatosConsulta.sitioGr, DatosConsulta.consMedio);

            vsalida.codSalida = salidaWsProcInt.codSalida.ToString();
            vsalida.datosSecad = salidaWsProcInt.datosSecad.ToString();
            vsalida.desSalida = salidaWsProcInt.descSalida.ToString();

            return vsalida;
        }

        public ClGrabacionesICP.ClDatosGrabacionOut MConsDatosGrabacion(ClGrabacionesICP.ClDatosGrabacionIn DatosConsGrabacion)
        {
            ClGrabacionesICP.ClDatosGrabacionOut vsalida = new ClGrabacionesICP.ClDatosGrabacionOut();

          
            ServProcesoIntegracion.ServSecadSoapClient ServIntegracion = new ServProcesoIntegracion.ServSecadSoapClient();

            ServProcesoIntegracion.ClDatosGrabacionOut salidaWs = new ServProcesoIntegracion.ClDatosGrabacionOut();
          
            //System.Diagnostics.EventLog.WriteEntry ("secad","acd: " + DatosGrabacion.acd);
            salidaWs = ServIntegracion.M3DatosGrabaciones(DatosConsGrabacion.sitioGr, DatosConsGrabacion.Llamada, DatosConsGrabacion.fechaInicial, DatosConsGrabacion.fechaFinal, DatosConsGrabacion.acd);

            vsalida.datosSecad = salidaWs.datosSecad.ToString();
            vsalida.codSalida = salidaWs.codSalida.ToString();
            vsalida.descSalida = salidaWs.descSalida.ToString();
            return vsalida;
        }


    }
}
