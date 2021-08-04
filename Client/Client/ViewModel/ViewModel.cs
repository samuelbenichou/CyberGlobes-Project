using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Model;


namespace Client.ViewModel
{
    public class ViewModelClass
    {
        public ServerStatus serverStatus;

        public List<ServerStatus> GetServerStatusList(out string lastUpdated)
        {
            List<Tuple<string, string>> listOfServers = new List<Tuple<string, string>> { new Tuple<string, string>("http://localhost:57325/Service1.svc","comp1"),
                                                                                          new Tuple<string, string>("http://localhost:57325/Service1.svc","comp3") };
            List<ServerStatus> serverStatusList = new List<ServerStatus>();
            foreach (var serverAddress in listOfServers)
            {
                // connect to service
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                EndpointAddress endpointAddress = new EndpointAddress(serverAddress.Item1);
                ServiceReference1.IService1 ADUser = null;
                try
                {
                    ADUser = new ChannelFactory<ServiceReference1.IService1>(basicHttpBinding, endpointAddress).CreateChannel();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed creating channel for server " + endpointAddress.Uri.ToString(), "Error Creating Channel");
                }

                try
                {
                    Dictionary<string, int> output = ADUser?.GetCompagnyActive(serverAddress.Item2);
                    if (output == null)
                    {
                        throw new Exception();
                    }
                    ServerStatus serverStatus = UpdateScreen(serverAddress.Item1, serverAddress.Item2, output);
                    serverStatusList.Add(serverStatus);
                }
                catch (Exception ex)
                {
                    serverStatusList.Add(new ServerStatus()
                    {
                        Status = Status.Down,
                        Server = serverAddress.Item1,
                        Company = serverAddress.Item2
                    });
                }
            }
            lastUpdated = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            return serverStatusList;
        }

        public ServerStatus UpdateScreen(string serverName, string compName, Dictionary<string, int> output)
        {
            Status status = Status.Green;
            foreach (KeyValuePair<string, int> item in output)
            {
                if (item.Value < 2)
                {
                    status = Status.Red;
                }
                else if (item.Value == 2 || item.Value == 3 && status != Status.Red)
                {
                    status = Status.Orange;
                }
            }
            serverStatus = new ServerStatus()
            {
                Status = status,
                Server = serverName,
                Company = compName,
                P1 = output["P1"],
                P2 = output["P2"],
                P3 = output["P3"],
                P4 = output["P4"],
                P5 = output["P5"],
                P6 = output["P6"],
                P7 = output["P7"],
                P8 = output["P8"],
                P9 = output["P9"],
            };

            return serverStatus;
        }
    }
}
