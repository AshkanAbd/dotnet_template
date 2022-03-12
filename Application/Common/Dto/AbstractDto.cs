namespace Application.Common.Dto;

public abstract class AbstractDto
{
    public AbstractDto()
    {
    }

    public abstract AbstractDto Parse(object obj);
}