## Raw XML

In certain integration scenarios it may be necessary to bypass NServiceBus opinionated serialization format (essentially key/value pairs) and directly send custom XML structures over messaging. In order to do that declare one or multiple properties on the message contract as `XDocument` or `XElement`.


### Message with XDocument

snippet: MessageWithXDocument


### Payload with XDocument

snippet: XDocumentPayload


### Message with XElement

snippet: MessageWithXElement


### Payload with XElement

snippet: XElementPayload


The caveat of this approach is that the serializer will wrap the data in an outer node being named after the name of the property. So in the examples above note the associated expected payloads.

To avoid that, for interoperability reasons, instruct the serializer to not wrap raw XML structures:

snippet: ConfigureRawXmlSerialization

NOTE: The name of the property on the message must exactly match the name of the root node in the XML structure in order to be able to correctly deserialize the no longer wrapped content.