﻿using GitHub.Models;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;

namespace MetricsServer
{
    public class Server
    {
        readonly string host;
        readonly int port;
        NancyHost server;
        public Server(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void Start()
        {
            var conf = new HostConfiguration { RewriteLocalhost = false };
            server = new NancyHost(conf, new Uri($"http://{host}:{port}"));
            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }
    }

    public class UsageModule : NancyModule
    {
        public UsageModule()
        {
            Post["/api/usage/visualstudio"] = p =>
            {
                var errors = new List<string>();
                var usage = this.Bind<UsageModel>();
                if (String.IsNullOrEmpty(usage.Dimensions.AppVersion))
                    errors.Add("Empty appVersion");
                Version result = null;
                if (!Version.TryParse(usage.Dimensions.AppVersion, out result))
                    errors.Add("Invalid appVersion");
                if (String.IsNullOrEmpty(usage.Dimensions.Lang))
                    errors.Add("Empty lang");
                if (String.IsNullOrEmpty(usage.Dimensions.VSVersion))
                    errors.Add("Empty vSVersion");
                if (usage.Dimensions.Date == DateTimeOffset.MinValue)
                    errors.Add("Empty date");
                if (usage.Measures.NumberOfStartups == 0)
                    errors.Add("Startups is 0");
                if (errors.Count > 0)
                {
                    return Negotiate
                        .WithStatusCode(HttpStatusCode.InternalServerError)
                        .WithAllowedMediaRange(MediaRange.FromString("application/json"))
                        .WithMediaRangeModel(
                              MediaRange.FromString("application/json"),
                              new { result = errors }); // Model for 'application/json';
                }

                return Negotiate
                    .WithAllowedMediaRange(MediaRange.FromString("application/json"))
                    .WithMediaRangeModel(
                            MediaRange.FromString("application/json"),
                            new { result = "Cool usage" }); // Model for 'application/json';
            };
        }
    }
}
