﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

using Cogito.Composition.Scoping;
using Cogito.Web;

using Nancy;
using Nancy.Responses;
using Nancy.Responses.Negotiation;

namespace Cogito.Nancy.Razor
{

    /// <summary>
    /// Processes responses which have available Nancy Razor views.
    /// </summary>
    [Export(typeof(IResponseProcessor))]
    public class NancyRazorViewProcessor : IResponseProcessor
    {

        static readonly ProcessorMatch noMatch = new ProcessorMatch();
        static readonly ProcessorMatch onMatch = new ProcessorMatch { ModelResult = MatchResult.DontCare, RequestedContentTypeResult = MatchResult.ExactMatch };

        INancyRazorViewProvider viewProvider;
        INancyRazorViewRenderer viewRenderer;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="composition"></param>
        [ImportingConstructor]
        public NancyRazorViewProcessor(
            [Import] INancyRazorViewProvider viewProvider,
            [Import] INancyRazorViewRenderer viewRenderer)
        {
            this.viewProvider = viewProvider;
            this.viewRenderer = viewRenderer;
        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { yield break; }
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return
                model != null &&
                requestedMediaRange.Matches("text/html") &&
                viewProvider.GetViews(context, requestedMediaRange, (object)model).Any() ?
                onMatch : noMatch;
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            // obtain view
            var view = viewProvider.GetViews(context, requestedMediaRange, (object)model)
                .FirstOrDefault();
            if (view == null)
                return new HtmlResponse(HttpStatusCode.InternalServerError);

            return new HtmlResponse(
                contents: stream =>
                {
                    using (var wrt = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                        viewRenderer.RenderView(context, requestedMediaRange, view.View, (object)model, wrt);
                });
        }

    }

}
