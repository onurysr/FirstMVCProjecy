﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITServiceApp.Models.Payment
{
    public class InstallmentModel
    {
        public string BinNumber { get; set; }
        public int  Commercial { get; set; }
        public string Price { get; set; }
        public string CardType { get; set; }
        public string CardAssociation { get; set; }
        public string CardFamilyName { get; set; }
        public int? Force3Ds { get; set; }
        public long? BlankCode { get; set; }
        public string BankName { get; set; }
        public int? ForceCvc { get; set; }

        public List<InstallmentPriceModel> InstallmentPrices { get; set; }
    }
}
