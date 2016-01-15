﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Net
{
	public class InMemoryConnection : IConnection
	{
		private readonly byte[] _responseBody;
		private readonly int _statusCode;

		public List<Tuple<string, Uri, PostData<object>>> Requests = new List<Tuple<string, Uri, PostData<object>>>(); 
		
		public InMemoryConnection() 
		{
			_statusCode = 200;
		}

		public InMemoryConnection(byte[] responseBody, int statusCode = 200) 
		{
			_responseBody = responseBody;
			_statusCode = statusCode;
		}

		public virtual Task<ElasticsearchResponse<TReturn>> RequestAsync<TReturn>(RequestData requestData) where TReturn : class =>
			Task.FromResult(this.ReturnConnectionStatus<TReturn>(requestData));

		public virtual ElasticsearchResponse<TReturn> Request<TReturn>(RequestData requestData) where TReturn : class => 
			this.ReturnConnectionStatus<TReturn>(requestData);

		protected ElasticsearchResponse<TReturn> ReturnConnectionStatus<TReturn>(RequestData requestData, byte[] responseBody = null, int? statusCode = null)
			where TReturn : class
		{
			var body = responseBody ?? _responseBody;
			var builder = new ResponseBuilder<TReturn>(requestData)
			{
				StatusCode = statusCode ?? this._statusCode,
				Stream = (body != null) ? new MemoryStream(body) : null
			};
			var cs = builder.ToResponse();
			return cs;
		}
	}
}
