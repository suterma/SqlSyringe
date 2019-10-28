#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

namespace SqlSyringe {
    /// <summary>Data about the injection request.</summary>
    public class InjectionRequest {
        public string ConnectionString { get; set; }

        public bool IsQuery { get; set; }

        public string SqlCommand { get; set; }
    }
}