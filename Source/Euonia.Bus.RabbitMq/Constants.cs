﻿using Newtonsoft.Json;

namespace Nerosoft.Euonia.Bus.RabbitMq;

internal class Constants
{
	public static readonly JsonSerializerSettings SerializerSettings = new()
	{
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		ConstructorHandling = ConstructorHandling.Default,
		MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
	};

	public class MessageHeaders
	{
		public const string CorrelationId = "x-correlation-id";
		public const string MessageId = "x-message-id";
		public const string MessageType = "x-message-type";
		public const string ContentType = "x-content-type";
		public const string ContentEncoding = "x-content-encoding";
		public const string DeliveryMode = "x-delivery-mode";
		public const string Priority = "x-priority";
		public const string ReplyTo = "x-reply-to";
		public const string Expiration = "x-expiration";
		public const string Timestamp = "x-timestamp";
		public const string Type = "x-type";
		public const string UserId = "x-user-id";
	}
}