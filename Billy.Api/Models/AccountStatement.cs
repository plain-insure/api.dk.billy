namespace Billy.Api.Models
{

    public class AccountStatementRoot
    {
        public Meta Meta { get; set; }
        public AccountStatement AccountStatement { get; set; }
    }


    public class AccountStatement
    {
        public int StartBalance { get; set; }
        public bool IsTruncated { get; set; }
        public Row[] Rows { get; set; }

        public class Row
        {
            public string Id { get; set; }
            public string TransactionId { get; set; }
            public int TransactionNo { get; set; }
            public string VoucherNo { get; set; }
            public string OriginatorName { get; set; }
            public string EntryDate { get; set; }
            public string Text { get; set; }
            public string BaseAmount { get; set; }
            public string CurrencyId { get; set; }
            public Currency Currency { get; set; }

            public int Tax { get; set; }
            public bool IsVoided { get; set; }
            public int GrossAmount { get; set; }
            public int Credit { get; set; }
            public int Balance { get; set; }
            public bool IsSpecial { get; set; }
        }
    }



}
