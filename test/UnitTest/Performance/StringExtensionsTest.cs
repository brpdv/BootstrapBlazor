﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest.Performance
{
    public class StringExtensionsTest : IClassFixture<ReadRazorFile>
    {
        private string Payload { get; set; }

        private List<KeyValuePair<string, string>> Localizers { get; }

        private ITestOutputHelper Logger { get; }

        private const int Count = 100;

        public StringExtensionsTest(ReadRazorFile reader, ITestOutputHelper logger)
        {
            Payload = reader.FileContent;
            Localizers = reader.Localizers;
            Logger = logger;
        }

        [Fact]
        public void Replace_Ok()
        {

        }

        [Fact]
        public void ReplaceString()
        {
            var sw = Stopwatch.StartNew();

            for (var index = 0; index < Count; index++)
            {
                Loop(Payload);
            }

            sw.Stop();
            Logger.WriteLine(sw.Elapsed.ToString());

            void Loop(string payload)
            {
                Localizers.ForEach(kv =>
                {
                    payload = payload.Replace($"@(((MarkupString)Localizer[\"{kv.Key}\"].Value).ToString())", kv.Value);
                    payload = payload.Replace($"@((MarkupString)Localizer[\"{kv.Key}\"].Value)", kv.Value);
                    payload = payload.Replace($"@Localizer[\"{kv.Key}\"]", kv.Value);
                });
                payload = payload.Replace("@@", "@");
                payload = payload.Replace("&lt;", "<");
                payload = payload.Replace("&gt;", ">");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReadRazorFile
    {
        public string FileContent { get; }

        public List<KeyValuePair<string, string>> Localizers { get; }

        public ReadRazorFile()
        {
            FileContent = ReadFile();
            Localizers = new List<KeyValuePair<string, string>>(BuildLocalizer());

            string ReadFile()
            {
                var dirSeparator = Path.DirectorySeparatorChar;
                var rootFolder = $"..{dirSeparator}..{dirSeparator}..{dirSeparator}..{dirSeparator}..{dirSeparator}";
                var razorFile = $"src{dirSeparator}BootstrapBlazor.Shared{dirSeparator}Samples{dirSeparator}Alerts.razor";
                var file = Path.Combine(AppContext.BaseDirectory, rootFolder, razorFile);
                return File.Exists(file) ? File.ReadAllText(file) : "";
            }

            IEnumerable<KeyValuePair<string, string>> BuildLocalizer()
            {
                var localizers = new List<KeyValuePair<string, string>>
                {
                    new("Title", "Alert 警告"),
                    new("SubTitle", "用于页面中展示重要的提示信息。"),
                    new("BaseUsageText", "基本用法"),
                    new("IntroText1", "页面中的非浮层元素，不会自动消失。"),
                    new("AlertPrimaryText", "主要的警告框"),
                    new("AlertSecondaryText", "次要的警告框"),
                    new("AlertSuccessText", "成功的警告框"),
                    new("AlertDangerText", "危险的警告框"),
                    new("AlertWarningText", "警告的警告框"),
                    new("AlertInfoText", "信息的警告框"),
                    new("AlertDarkText", "黑暗的警告框"),
                    new("CloseButtonUsageText", "关闭按钮"),
                    new("IntroText2", "提供关闭按钮的警告框"),
                    new("WithIconUsageText", "带 Icon"),
                    new("IntroText3", "表示某种状态时提升可读性。"),
                    new("ShowBarUsageText", "显示左侧 Bar"),
                    new("IntroText4", "作为 <code>Tip</code> 使用")
                };
                return localizers;
            }
        }
    }
}
