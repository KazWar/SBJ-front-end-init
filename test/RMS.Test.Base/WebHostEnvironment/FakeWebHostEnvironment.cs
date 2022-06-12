using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMS.Test.Base.WebHostEnvironment
{
    class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string WebRootPath { get => "C:\\ST\\rms2.0\\test\\RMS.Test.Base\\WebHostEnvironment\\root"; set => throw new NotImplementedException(); }
        public string ApplicationName { get => "RMS Testing"; set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get => "C:\\ST\\rms2.0\\test\\RMS.Test.Base\\WebHostEnvironment\\root"; set => throw new NotImplementedException(); }
        public string EnvironmentName { get => "Testing"; set => throw new NotImplementedException(); }
    }
}
