﻿using Orchard.ContentManagement.Records;

namespace NGM.Forum.Models {
    public class ThreadPartRecord : ContentPartRecord {
        public virtual bool IsClosed { get; set; }
        public virtual int PostCount { get; set; }
    }
}