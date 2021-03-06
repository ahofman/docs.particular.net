﻿using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;

#region sagaPhase2

[SqlSaga(
     correlationProperty: nameof(OrderSagaData.OrderNumber),
     transitionalCorrelationProperty: nameof(OrderSagaData.OrderId)
 )]
public class OrderSaga :
    SqlSaga<OrderSagaData>,
    IAmStartedByMessages<StartOrder>
{
    static ILog log = LogManager.GetLogger<OrderSaga>();

    protected override void ConfigureMapping(MessagePropertyMapper<OrderSagaData> mapper)
    {
        mapper.MapMessage<StartOrder>(_ => _.OrderNumber);
    }

    public Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
        Data.OrderNumber = message.OrderNumber;
        log.Info($"Received StartOrder message. OrderId={Data.OrderId}. OrderNumber={Data.OrderNumber}");
        return Task.CompletedTask;
    }
}

#endregion