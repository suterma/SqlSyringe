using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSyringe
{
    /// <summary>
    /// Data about the injection request.
    /// </summary>
    public class InjectionRequest
    {
        public string ConnectionString { get; set; }

        public bool IsQuery { get; set; }

        public string SqlCommand { get; set; }
    }
}
