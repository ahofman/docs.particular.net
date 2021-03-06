---
title: Outbox
summary: Reliable messaging without distributed transactions
reviewed: 2016-08-03
component: Core
versions: "[5,)"
tags:
- DTC
redirects:
- nservicebus/no-dtc
related:
- samples/outbox
- nservicebus/nhibernate/outbox
- nservicebus/service-fabric/outbox
- nservicebus/ravendb/outbox
---

Outbox allows endpoints to run with similar reliability to DTC while not actually using DTC.


## How it works

The Outbox feature has been implemented using the [Outbox](http://gistlabs.com/2014/05/the-outbox/) and the [Deduplication](https://en.wikipedia.org/wiki/Data_deduplication#In-line_deduplication) patterns.

Every time a message is processed, a copy of that message is stored in the persistent _Outbox storage_. Whenever a new message is received, the framework verifies if that message has been processed already by checking if it's present in the Outbox storage.

If the message is not found in the Outbox storage, then it is processed in a regular way as shown in the following diagram:

![No DTC Diagram](outbox.svg)

Processing a new incoming message consists of the following steps:

 * The handlers processing that message are invoked.
 * The _downstream messages_ (messages sent during processing the message, e.g. from inside the handlers) are stored in a durable storage and business data is saved. Both operations are executed within a single transaction.
 * The downstream messages are sent and marked as dispatched.

If the message is found in the Outbox storage, then it is treated as a duplicate and not processed again. However, even though the message has been processed and business data has been saved, framework might fail to send downstream messages. If the downstream messages are not marked as dispatched, they will be dispatched again.

Note: On the wire level the Outbox guarantees `at-least-once` message delivery, meaning that the downstream messages can be sent and processed multiple times. At the handler level, however, the Outbox guarantees `exactly-once` message processing, similar to distributed transactions. This higher guarantee level is due to the deduplication that happens on the receiving side.


## Important design considerations

 * The business data and deduplication data need to be stored in the same database.
 * The Outbox feature works only for messages sent from NServiceBus message handlers.
 * Endpoints using DTC can communicate with endpoints using Outbox only if either of the following conditions are satisfied:
   * Endpoints using Outbox don't send messages to endpoints using DTC. However, endpoints using DTC can send messages to endpoints using Outbox.
   * If endpoints using Outbox send messages to endpoints using DTC, then the handlers processing those messages are [idempotent](https://en.wikipedia.org/wiki/Idempotence).
 * The Outbox may generate duplicate messages if outgoing messages are successfully dispatched but the _Mark as dispatched_ phase fails. This may happen for a variety of reasons, including _Outbox storage_ connectivity issues and deadlocks.


## Enabling the Outbox

partial: enable-outbox

To learn about Outbox configuration options such as time to keep deduplication data or deduplication data clean up interval, refer to the dedicated pages for [NHibernate](/nservicebus/nhibernate/outbox.md), [RavenDB](/nservicebus/ravendb/outbox.md) or [ServiceFabric](/nservicebus/service-fabric/outbox.md) persistence.


## Persistence

The Outbox feature requires persistence in order to store the messages and enable deduplication.

Refer to the dedicated pages for [NHibernate](/nservicebus/nhibernate/outbox.md), [RavenDB](/nservicebus/ravendb/outbox.md) or [ServiceFabric](/nservicebus/service-fabric/outbox.md) persistence.
