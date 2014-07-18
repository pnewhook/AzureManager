using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute.Models;

namespace AzureManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SubscriptionCloudCredentials _credentials;
        public MainWindow()
        {
            InitializeComponent();
            var x509Store = new X509Store(StoreName.My);
            x509Store.Open(OpenFlags.ReadOnly);
            var certCollection = x509Store.Certificates.Find(
                X509FindType.FindByThumbprint,
                "dbc5ed494d6a122705679aa10805d6e9e43a82c2",
                false);
            x509Store.Close();
            var x509Certificate2 = certCollection[0];
            _credentials = new CertificateCloudCredentials("ac36d589-3879-4081-bdf4-84a79bf79877", x509Certificate2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var computeManagementClient = CloudContext.Clients.CreateComputeManagementClient(_credentials);
            var detailed = computeManagementClient.HostedServices.GetDetailed("pearson-ccsoc-vndr");
            Console.WriteLine(detailed.ServiceName);
            var stagingDeployment = detailed.Deployments.Single(d => d.DeploymentSlot == DeploymentSlot.Staging);

            var swapParama = new DeploymentSwapParameters { SourceDeployment = stagingDeployment.Name };

            computeManagementClient.Deployments.BeginSwapping(detailed.ServiceName, swapParama);
        }

    }
}
