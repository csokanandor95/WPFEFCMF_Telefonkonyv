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
    public partial class Szam {

        public Szam()
        {
            this.Személyek = new List<Szemely>();
            OnCreated();
        }

        public string SzámId { get; set; }

        public string SzámSztring { get; set; }

        public virtual IList<Szemely> Személyek { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
