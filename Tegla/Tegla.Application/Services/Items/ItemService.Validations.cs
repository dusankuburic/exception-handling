using Tegla.Domain.Models.Items;
using Tegla.Domain.Models.Items.Exceptions;

namespace Tegla.Application.Services.Items;

public partial class ItemService
{
    public void ValidateItemOnCreate(Item item)
    {
        ValidateItem(item);
        ValidateItemStrings(item);
        ValidateItemPrice(item);
    }

    public void ValidateItemOnUpdate(Item item)
    {
        ValidateItem(item);
        ValidateItemStrings(item);
        ValidateItemPrice(item);
    }

    public void ValidateItem(Item item)
    {
        if(item is null)
        {
            throw new NullItemException();
        }
    }

    public void ValidateItemStrings(Item item)
    {
        switch (item)
        {
            case { } when IsInvalid(item.Name):
                throw new InvalidItemException(
                    parameterName: nameof(item.Name),
                    parameterValue: item.Name);

            case { } when IsInvalid(item.Description):
                throw new InvalidItemException(
                    parameterName: nameof(item.Description),
                    parameterValue: item.Description);

            case { } when IsInvalid(item.Make):
                throw new InvalidItemException(
                    parameterName: nameof(item.Make),
                    parameterValue: item.Make);

            case { } when IsInvalid(item.Origin):
                throw new InvalidItemException(
                    parameterName: nameof(item.Origin),
                    parameterValue: item.Origin);
        }
    }

    public void ValidateItemPrice(Item item)
    {
        switch(item)
        {
            case { } when IsInvalid(item.Price):
                throw new InvalidItemException(
                    parameterName: nameof(item.Price),
                    parameterValue: item.Price);
        }
    }

    public bool IsInvalid(string input) =>
        string.IsNullOrWhiteSpace(input);

    public bool IsInvalid(double input) =>
        input >= default(double) ? false : true;
}
