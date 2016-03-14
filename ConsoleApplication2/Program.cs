using Bss.Remoting;
using Bss.Services.SmartDrv;
using Bss.Services.SmartDrv.Common.Device;
using Bss.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace ConsoleApplication2
{
    class Program
    {

        static void Main(string[] args)
        {
            string pi = GetZtpService();
        }

        static RemoteClient _rClient = null;
        static void Est()
        {
            _rClient = new RemoteClient();
            _rClient.Connect("10.208.4.11", 10812, new System.Net.NetworkCredential("oper", "oper"), System.Net.Security.ProtectionLevel.None,
               Bss.Sys.TypeAuthenticate.System);

        }

        static ItemDef[] FillListDef(PointItems[] items)
        {
            List<ItemDef> lst = new List<ItemDef>();
            for (int i = 0; i < items.Length; i++)
            {
                Bss.Sys.ItemDef idef = new Bss.Sys.ItemDef();
                idef.active = true;
                idef.hClient = i;
                idef.itemID = items[i].CommState.GUID;
                lst.Add(idef);
            }
            return lst.ToArray();
        }

        static PointItems[] Deserelize(string data)
        {
            PointItems[] pi = null;
            System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(PointItems[]));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                byte[] bytes = Encoding.GetEncoding(1251).GetBytes(data);
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                pi = (PointItems[])ser.ReadObject(ms);
            }
            return pi;
        }
        static string GetZtpService()
        {
            string data = string.Empty;
            Est();
            if (_rClient.IsConnected)
            {
                ISerManCallBack serman = _rClient.GetRemoteObject().GetSerMan();
                var reg = serman.GetRegService();
                Bss.Sys.RegSrv.SERVICE_INFO[] asi = serman.GetRegService().GetServiceListFromServiceType("SmartDrvSrv");
                foreach (Bss.Sys.RegSrv.SERVICE_INFO si in asi)
                {
                    Bss.Sys.RegSrv.MODULE_INFO[] ami = reg.GetModuleList(si.serviceFullName);
                    for (int i = 0; i < ami.Length; i++)
                    {
                        IEnterpriseInfo rr = (IEnterpriseInfo)serman.GetService(ami[i].moduleFullName, typeof(IEnterpriseInfo).Name);
                        if (rr != null)
                        {
                            string[] strarray = rr.EnterpriseInfo();
                            if (strarray.Length > 0)
                                data = strarray[0];
                        }
                    }
                }
            }
            return data;
        }
    }
}


[DataContract]
public class PointItems
{

    [DataMember(Order = 1)]
    public string ID { get; set; }
    [DataMember(Order = 2)]
    public string IP { get; set; }
    [DataMember(Order = 3)]
    public int Address { get; set; }
    [DataMember(Order = 4)]
    public JsonDataLevel CommState { get; set; }
    [DataMember(Order = 5)]
    public JsonDataLevel[] DOUT { get; set; }
    [DataMember(Order = 6)]
    public JsonDataLevel[] DIN { get; set; }
    [DataMember(Order = 7)]
    public JsonDataLevel[] AIN { get; set; }

}

[DataContract]
public class JsonDataLevel
{
    [DataMember(Order = 1)]
    public string Name { get; set; }
    [DataMember(Order = 2)]
    public int RegId { get; set; }
    [DataMember(Order = 3)]
    public string GUID { get; set; }
}

