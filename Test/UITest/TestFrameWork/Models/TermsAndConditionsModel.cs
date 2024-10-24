﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TestFrameWork.Models
{
    public class TermsAndConditionsModel
    {
        //Switching to JS
        //public static string TermsAndCondititionsCheckBox { get => "body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"; }

        public static string ContinueButton { get => "continue-btn"; }

        public static string TermsAndCondititionsCheckBox { get => @"document.querySelector(""body > app-root > app-layout > div.content > app-terms-and-conditions > p-card > div > div.p-card-body > div > div > div.checkbox-container > p-checkbox > div > div.p-checkbox-box"").click()"; }

    }
}
