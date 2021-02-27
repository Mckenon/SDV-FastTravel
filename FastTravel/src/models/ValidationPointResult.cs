namespace FastTravel.src.models
{
    public struct ValidationPointResult
    {
        public string messageKeyId { get; }
        public bool isValid { get; }
        
        public ValidationPointResult(bool isValid, string messageKeyId)
        {
            this.messageKeyId = messageKeyId;
            this.isValid = isValid;
        }
    }
}
