﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2025. 04. 13. 20:40:57
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace Telefonkonyv
{
    public partial class Szemely {

        public Szemely()
        {
            this.Számok = new List<Szam>();
            OnCreated();
        }

        public short SzemélyId { get; set; }

        public string Név { get; set; }

        public string Cím { get; set; }

        public string HelysegId { get; set; }

        public virtual Helyseg Helyseg { get; set; }

        public virtual IList<Szam> Számok { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
