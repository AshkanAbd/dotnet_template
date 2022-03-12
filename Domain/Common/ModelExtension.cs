namespace Domain.Common;

public abstract class ModelExtension : SoftDeletes.ModelTools.ModelExtension
{
    public long Id { get; set; }
}