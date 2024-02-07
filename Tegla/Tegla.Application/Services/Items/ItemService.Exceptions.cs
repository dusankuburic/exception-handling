using Microsoft.Data.SqlClient;
using Tegla.Application.Services.Items.Models;
using Tegla.Domain.Models.Items;
using Tegla.Domain.Models.Items.Exceptions;

namespace Tegla.Application.Services.Items;

public partial class ItemService
{
    public delegate ValueTask<CreateItemResponse> ReturningAddItemFunc();
    public delegate ValueTask<IEnumerable<Item>> ReturningListAllItemsFunc();
    public delegate ValueTask<UpdateItemResponse> ReturningModifyItemFunc();
    public delegate ValueTask<RetriveItemByIdResponse> ReturningRetriveItemByIdFunc();
    public delegate ValueTask<RemoveItemByIdResponse> ReturningRemoveItemByIdFunc();

    public async ValueTask<CreateItemResponse> TryCatch(ReturningAddItemFunc returningAddItemFunc)
    {
        try
        {
            return await returningAddItemFunc();
        }
        catch (NullItemException nullItemException)
        {
            throw CreateAndLogValidationException(nullItemException);
        }
        catch (InvalidItemException invalidItemException)
        {
            throw CreateAndLogValidationException(invalidItemException);
        }
        catch (SqlException sqlException)
        {
            throw CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<IEnumerable<Item>> TryCatch(ReturningListAllItemsFunc returningListAllItemsFunc)
    {
        try
        {
            return await returningListAllItemsFunc();
        }
        catch (SqlException sqlException)
        {
            throw CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<UpdateItemResponse> TryCatch(ReturningModifyItemFunc returningModifyItemFunc)
    {
        try
        {
            return await returningModifyItemFunc();
        }
        catch (NullItemException nullItemException)
        {
            throw CreateAndLogValidationException(nullItemException);
        }
        catch (InvalidItemException invalidItemException)
        {
            throw CreateAndLogValidationException(invalidItemException);
        }
        catch (SqlException sqlException)
        {
            throw CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<RetriveItemByIdResponse> TryCatch(ReturningRetriveItemByIdFunc returningRetriveItemByIdFunc)
    {
        try
        {
            return await returningRetriveItemByIdFunc();
        }
        catch (NullItemException nullItemException)
        {
            throw CreateAndLogValidationException(nullItemException);
        }
        catch (SqlException sqlException)
        {
            throw CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw CreateAndLogServiceException(exception);
        }
    }


    public async ValueTask<RemoveItemByIdResponse> TryCatch(ReturningRemoveItemByIdFunc returningRemoveItemByIdFunc)
    {
        try
        {
            return await returningRemoveItemByIdFunc();
        }
        catch (NullItemException nullItemException)
        {
            throw CreateAndLogValidationException(nullItemException);
        }
        catch (SqlException sqlException)
        {
            throw CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw CreateAndLogServiceException(exception);
        }
    }


    private ItemValidationException CreateAndLogValidationException(Exception exception)
    {
        var itemValidationException = new ItemValidationException(exception);

        _loggingBroker.LogError(itemValidationException);

        return itemValidationException;
    }

    private ItemDependencyException CreateAndLogCriticalDependencyException(Exception exception)
    {
        var itemDependencyException = new ItemDependencyException(exception);

        _loggingBroker.LogCritical(itemDependencyException);

        return itemDependencyException;
    }

    private ItemServiceException CreateAndLogServiceException(Exception exception)
    {
        var itemServiceException = new ItemServiceException(exception);

        _loggingBroker.LogError(itemServiceException);

        return itemServiceException;
    }
}
