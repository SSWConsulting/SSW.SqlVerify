﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSW.SqlVerify.Core
{
    public interface ISqlVerify
    {

        void UpdateDb();

        bool VerifyDb();

    }
}
