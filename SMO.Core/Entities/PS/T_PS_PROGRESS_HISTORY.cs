﻿using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PROGRESS_HISTORY : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string ACTION { get; set; }
        public virtual string PRE_STATUS { get; set; }
        public virtual string DES_STATUS { get; set; }
        public virtual string ACTOR { get; set; }
        public virtual string NOTE { get; set; }
        public virtual string TAB_NAME { get; set; }
    }
}
