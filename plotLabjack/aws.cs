using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Windows.Forms;

namespace plotLabjack
{
    internal class aws
    {
        MqttClient client = null;
        public aws()
        {
            var broker = "avdz0tx0oxt1t-ats.iot.eu-central-1.amazonaws.com"; //<AWS-IoT-Endpoint>           
            var port = 8883;
            var certPass = "12345";
            var clientId = "awsLabjack";
            //certificates Path
            var certificatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "certs");

            var caCertPath = Path.Combine(certificatesPath, "AmazonRootCA1.pem");

            var caCert = X509Certificate.CreateFromCertFile(caCertPath);

            var deviceCertPath = Path.Combine(certificatesPath, "certificate.cert.pfx");
            var deviceCert = new X509Certificate(deviceCertPath, certPass);


            // Create a new MQTT client.
            try
            {
                client = new MqttClient(broker, port, true, caCert, deviceCert, MqttSslProtocols.TLSv1_2);
                client.Connect(clientId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void sendMessage(Values adVal)
        {
            jsonADvalues values = new jsonADvalues();
            values.valueAD1 = adVal.readLastValueChannel1();
            values.valueAD2 = adVal.readLastValueChannel2();
            values.valueAD3 = adVal.readLastValueChannel3();
            values.valueAD4 = adVal.readLastValueChannel4();

            string output = JsonConvert.SerializeObject(values);
            //client.Publish("labjackValues", Encoding.UTF8.GetBytes(output));
        }

        private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine($"Successfully subscribed to the AWS IoT topic.");
        }
        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message received: " + Encoding.UTF8.GetString(e.Message));
        }
    }

    public class jsonADvalues
    {
        public double valueAD1 { get; set; }
        public double valueAD2 { get; set; }
        public double valueAD3 { get; set; }
        public double valueAD4 { get; set; }
    }
}