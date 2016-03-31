﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/");
        /*
        [TestMethod]
        public void TestMethod1()
        {
            Response r = client.DoGetAsync("/numbers?length={0}", "5").Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual(5, r.Data.Count);
            r = client.DoGetAsync("/numbers?length={0}", "-5").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void TestMethod2()
        {
            List<int> list = new List<int>();
            list.Add(15);
            Response r = client.DoPostAsync("/first", list).Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual(15, r.Data);
        }
        */
        [TestMethod]
        public void TestMethod0()
        {
            dynamic p1 = new ExpandoObject();
            p1.Nickname = "    ";

            Response r1 = client.DoPostAsync("/users", p1).Result;

            Assert.AreEqual(Forbidden, r1.Status);

        }
        [TestMethod]
        public void TestMethod1()
        {
            dynamic p1 = new ExpandoObject();
            p1.Nickname = "\t\t\n";

            Response r1 = client.DoPostAsync("/users", p1).Result;

            Assert.AreEqual(Forbidden, r1.Status);

        }
        [TestMethod]
        public void TestMethod3()
        {
            dynamic p1 = new ExpandoObject();
            dynamic p2 = new ExpandoObject();
            dynamic game = new ExpandoObject();
            p1.Nickname = "Mark";
            p2.Nickname = "Bob";

            Response r1 = client.DoPostAsync("/users", p1).Result;
            Response r2 = client.DoPostAsync("/users", p2).Result;

            p1.UserToken = r1.Data.UserToken;
            p2.UserToken = r2.Data.UserToken;

            p1.TimeLimit = "120";
            p2.TimeLimit = "120";

            r1 = client.DoPostAsync("/games", p1).Result;
            r2 = client.DoPostAsync("/games", p2).Result;
            
            Assert.AreEqual(Accepted, r1.Status);
            Assert.AreEqual(Created, r2.Status);

            p1.GameID = r1.Data.GameID;
            p2.GameID = r2.Data.GameID;

            game = client.DoGetAsync("/games/" + p1.GameID).Result;
            BoggleBoard board = new BoggleBoard(game.Data.Board.ToString());

            HashSet<string> testDictionary = new HashSet<string>();
            List<string> potentialWords = new List<string>();
            foreach (string i in File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "../../../\\dictionary.txt"))
            {
                testDictionary.Add(i);
                if (board.CanBeFormed(i))
                {
                    potentialWords.Add(i);
                }
            }
            Random rand = new Random();

            dynamic p1words = new ExpandoObject();
            dynamic p2words = new ExpandoObject();

            p1words.UserToken = p1.UserToken;
            

            for (int i = 0; i < 5; i++)
            {
                p1.Word = potentialWords[rand.Next(0,potentialWords.Count)];
                p1words.Word = p1.Word;
                r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;
                Assert.AreEqual(OK, r1.Status);
            }
        }
    }
}
