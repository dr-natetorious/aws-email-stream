using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailServiceLambda
{
    /// <summary>
    /// Represents an Item in the DynamoDb Table.
    /// </summary>
    public class EmailAudit
    {
        [DynamoDBHashKey]
        public string PartitionKey { get; set; }

        [DynamoDBRangeKey]
        public string SortKey { get; set; }

        [DynamoDBProperty(StoreAsEpoch = true)]
        public DateTime Expiration { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(7);

        [DynamoDBProperty]
        public string Contents { get; set; }
    }
}
