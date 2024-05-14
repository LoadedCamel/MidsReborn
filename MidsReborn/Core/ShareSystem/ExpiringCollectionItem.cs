namespace Mids_Reborn.Core.ShareSystem
{
    internal class ExpiringCollectionItem
    {
        // From DTO
        public string? Name { get; set; }
        public string? Archetype { get; set; }
        public string? Description { get; set; }
        public string? Primary { get; set; }
        public string? Secondary { get; set; }
        
        // From TransactionResult
        public string? Id { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? SchemaUrl { get; set; }
        public string? ExpiresAt { get; set; }

        // Added Property From When Shared
        public string? SharedOn { get; set; }
    }
}
