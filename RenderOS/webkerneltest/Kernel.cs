using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Network;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4;
using Cosmos.HAL;
using HtmlAgilityPack;
using Cosmos.System.Graphics;
using webkerneltest.HTMLRENDERV2;
using System.Drawing;
using CosmosTTF;
using IL2CPU.API.Attribs;
using Cosmos.Core.Memory;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System.IO;

namespace webkerneltest
{
    public class Kernel : Sys.Kernel
    {

        [ManifestResourceStream(ResourceName = "webkerneltest.LocalResources.skk.bmp")]
        static byte[] skk;
        [ManifestResourceStream(ResourceName = "webkerneltest.LocalResources.cc.bmp")]
        static byte[] cc;
        [ManifestResourceStream(ResourceName = "webkerneltest.LocalResources.ok.bmp")]
        static byte[] ok;

        DnsClient dnsClient;
        TcpClient tcpClient;
        Canvas canv;

        HtmlRenderer htmlRenderer;
        string htmlcode = "";

        [ManifestResourceStream(ResourceName = "webkerneltest.UbuntuMono-Regular.ttf")]
        static byte[] font;

        CosmosVFS vfs;

        Bitmap tbmp;

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");

            vfs = new CosmosVFS();
            VFSManager.RegisterVFS(vfs);

            using (var xClient = new DHCPClient())
            {
                /** Send a DHCP Discover packet **/
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
            }

            try
            {

                dnsClient = new DnsClient();
                tcpClient = new TcpClient();

                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk("szymekk.pl");

                Address address = dnsClient.Receive();
                dnsClient.Close();

                // tcp
                tcpClient.Connect(address, 80);

                // httpget
                string httpget = "GET /RadianceOS.html HTTP/1.1\r\n" +
                                 "User-Agent: RadianceOS\r\n" +
                                 "Accept: */*\r\n" +
                                 "Accept-Encoding: identity\r\n" +
                                 "Host: szymekk.pl\r\n" +
                                 "Connection: Keep-Alive\r\n\r\n";

                tcpClient.Send(Encoding.ASCII.GetBytes(httpget));

                // get http response
                var ep = new EndPoint(Address.Zero, 0);
                var data = tcpClient.Receive(ref ep);
                tcpClient.Close();


                string httpresponse = Encoding.ASCII.GetString(data);


                string[] responseParts = httpresponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                if (responseParts.Length == 2)
                {
                    string headers = responseParts[0];
                    string content = responseParts[1];
                    Console.WriteLine(content);
                    htmlcode = content;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                var css = @"
    
        body {
            background-color: #6a538c;
        }
        .testDiv {
            text-align: center;
         color: #cf9999;
        }
    
";
                var stylesheet = CssParser.Parse(css);
                foreach (var item in stylesheet.rawrules)
                {

                    Console.WriteLine(item.Key);

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }

        private static void PrintAllNodes(HtmlNode node)
        {
            Console.WriteLine("Node Name: " + node.Name + "\n" + node.InnerHtml);

            foreach (var childNode in node.ChildNodes)
            {
                PrintAllNodes(childNode);
            }
        }

        private static void RenderNode(HtmlNode node)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Document:
                    RenderDocument(node);
                    break;

                case HtmlNodeType.Element:
                    RenderElement(node);
                    break;

                case HtmlNodeType.Text:
                    RenderText(node);
                    break;

                case HtmlNodeType.Comment:
                    RenderComment(node);
                    break;
            }
        }

        private static void RenderDocument(HtmlNode node)
        {
            // Render the start of the document.
            Console.WriteLine("Start Document");

            // Render all child nodes.
            foreach (var childNode in node.ChildNodes)
            {
                RenderNode(childNode);
            }

            // Render the end of the document.
            Console.WriteLine("End Document");
        }

        private static void RenderElement(HtmlNode node)
        {
            // Render the start of an element.
            Console.WriteLine("Start Element: " + node.Name);

            // Render all attributes.
            foreach (var attribute in node.Attributes)
            {
                Console.WriteLine("Attribute: " + attribute.Name + " = " + attribute.Value);
            }

            // Render all child nodes.
            foreach (var childNode in node.ChildNodes)
            {
                RenderNode(childNode);
            }

            // Render the end of an element.
            Console.WriteLine("End Element: " + node.Name);
        }

        private static void RenderText(HtmlNode node)
        {
            // Render a text node.
            Console.WriteLine("Text: " + node.InnerHtml);
        }

        private static void RenderComment(HtmlNode node)
        {
            // Render a comment node.
            Console.WriteLine("Comment: " + node.InnerHtml);
        }

        protected override void Run()
        {
            Console.Write("command: ");
            var input = Console.ReadLine().Split(' ');
            switch (input[0].ToLower())
            {

                case "ip":

                    Console.WriteLine(NetworkConfiguration.CurrentAddress.ToString());

                    break;
                case "parse":

                    var html = @"<html>
                        <body>
                            <h1>Welcome to my website</h1>
                            <div id='mainContent'>
                                <p>This is some text</p>
                            </div>
                        </body>
                    </html>";

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    PrintAllNodes(htmlDoc.DocumentNode);

                    break;

                case "render":

                    htmlRenderer = new HtmlRenderer(1280, 720);

                    htmlRenderer.Render(htmlcode);
                    break;
                case "render2":

                    HtmlRender2.AddResource(@"skk.png",skk);
                    HtmlRender2.AddResource(@"ok.png",ok);
                    HtmlRender2.AddResource(@"https://i.creativecommons.org/l/by-nd/4.0/80x15.png", cc);

                    canv = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
                    try
                    {

                        RadianceOS.Render.Canvas.setup(canv);

                        TTFManager.RegisterFont("UMR", font);

                        canv.Clear(Color.Blue);
                        canv.Display();
                        canv.RenderHTML(htmlcode);
                        canv.Display();
                    }
                    catch (Exception ex)
                    {

                        canv.Disable();
                        Console.WriteLine(ex.Message);
                    }

                    //var html1 = @"<html>
                    //    <body>
                    //        <h1>Welcome to my website</h1>
                    //        <div id='mainContent'>
                    //            <p>This is some text</p>
                    //        </div>
                    //    </body>
                    //</html>";

                    //var htmlDoc1 = new HtmlDocument();
                    //htmlDoc1.LoadHtml(html1);

                    //RenderNode(htmlDoc1.DocumentNode);

                    break;

                case "request":

                    try
                    {

                        dnsClient = new DnsClient();
                        tcpClient = new TcpClient();

                        dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                        dnsClient.SendAsk(input[1]);

                        Address address = dnsClient.Receive();
                        dnsClient.Close();

                        // tcp
                        tcpClient.Connect(address, 80);

                        // httpget
                        string httpget = $"GET /{input[2]} HTTP/1.1\r\n" +
                                         "User-Agent: RadianceOS\r\n" +
                                         "Accept: */*\r\n" +
                                         "Accept-Encoding: identity\r\n" +
                                         $"Host: {input[1]}\r\n" +
                                         "Connection: Keep-Alive\r\n\r\n";

                        tcpClient.Send(Encoding.ASCII.GetBytes(httpget));

                        // get http response
                        var ep = new EndPoint(Address.Zero, 0);
                        var data = tcpClient.Receive(ref ep);
                        tcpClient.Close();


                        string httpresponse = Encoding.ASCII.GetString(data);


                        string[] responseParts = httpresponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                        if (responseParts.Length == 2)
                        {
                            string headers = responseParts[0];
                            string content = responseParts[1];
                            Console.WriteLine(content);
                            htmlcode = content;
                        }

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                    break;
                case "img":

                    try
                    {

                        try
                        {
                            canv = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
                            RadianceOS.Render.Canvas.setup(canv);

                            TTFManager.RegisterFont("UMR", font);

                            canv.Clear(Color.White);

                            canv.DrawImage(tbmp,0,0);

                            canv.Display();
                            Heap.Collect();
                        }
                        catch (Exception ex)
                        {

                            //canv.Disable();
                            Console.WriteLine(ex.Message);

                        }

                        //tcpClient.Close();

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                    break;
                case "pm":

                    try
                    {

                        dnsClient = new DnsClient();
                        tcpClient = new TcpClient();

                        dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                        dnsClient.SendAsk("fuebfewfewf.byethost8.com");

                        Address address = dnsClient.Receive();
                        dnsClient.Close();

                        // tcp
                        tcpClient.Connect(address, 80);

                        // httpget
                        string httpget = $"GET /test.html HTTP/1.1\r\n" +
                                         "User-Agent: RadianceOS\r\n" +
                                         "Accept: */*\r\n" +
                                         "Accept-Encoding: identity\r\n" +
                                         $"Host: fuebfewfewf.byethost8.com\r\n" +
                                         "Connection: Keep-Alive\r\n\r\n";

                        tcpClient.Send(Encoding.ASCII.GetBytes(httpget));

                        // get http response
                        var ep = new EndPoint(Address.Zero, 0);
                        var data = tcpClient.Receive(ref ep);
                        tcpClient.Close();


                        string httpresponse = Encoding.ASCII.GetString(data);


                        string[] responseParts = httpresponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                        if (responseParts.Length == 2)
                        {
                            string headers = responseParts[0];
                            string content = responseParts[1];
                            Console.WriteLine(content);
                            htmlcode = content;
                        }

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                    break;

                default:
                    break;
            }
            Heap.Collect();
        }
    }
}
