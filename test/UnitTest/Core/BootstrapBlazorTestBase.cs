﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace UnitTest.Core
{
    [Collection("BlazorTestContext")]
    public class BootstrapBlazorTestBase
    {
        protected TestContext Context { get; }

        public BootstrapBlazorTestBase()
        {
            Context = BootstrapBlazorTestHost.Instance;
        }
    }

    [CollectionDefinition("BlazorTestContext")]
    public class BootstrapBlazorTestCollection : ICollectionFixture<BootstrapBlazorTestHost>
    {

    }

    public class BootstrapBlazorTestHost : IDisposable
    {
        [NotNull]
        internal static TestContext? Instance { get; private set; }

        public BootstrapBlazorTestHost()
        {
            Instance = new TestContext();
            Instance.Services.AddBootstrapBlazor();
            Instance.Services.AddConfiguration();
            Instance.Services.RegisterProvider();
            Instance.JSInterop.Mode = JSRuntimeMode.Loose;
        }

        public void Dispose()
        {
            Instance.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
