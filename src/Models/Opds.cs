public class OpdsConstants
{
    public class MediaTypes
    {
        public static readonly string Gif = "image/gif";
        
        public static readonly string Png = "image/png";
        
        public static readonly string Jpeg = "image/jpeg";
        
        public static readonly string Epub = "application/epub+zip";
        
        public static readonly string Mobi = "application/x-mobipocket-ebook";
    }
    
    public class FeedTypes
    {
        /// <summary>
        /// The link type for an acquisition feed.
        /// </summary>
        public static readonly string Acquisition = "application/atom+xml;profile=opds-catalog;kind=acquisition";

        /// <summary>
        /// The link type for a navigation feed.
        /// </summary>
        public static readonly string Navigation = "application/atom+xml;profile=opds-catalog;kind=navigation";

        public static readonly string Search = "application/opensearchdescription+xml";
    }
    
    public class Relations
    {
        /// <summary>
        /// The root of the catalog.
        /// </summary>
        public static readonly string Start = "start";
        
        /// <summary>
        /// A generic subsection of the catalog
        /// </summary>
        public static readonly string Subsection = "subsection";
        
        /// <summary>
        /// A subsection of content previously acquired by the user.
        /// </summary>
        public static readonly string Shelf = "http://opds-spec.org/shelf";
        
        /// <summary>
        /// A subsection of content previously subscribed to by the user.
        /// </summary>
        public static readonly string Subscriptions = "http://opds-spec.org/subscriptions";

        /// <summary>
        /// A subsection of content listed most recent first.
        /// </summary>
        public static readonly string New = "http://opds-spec.org/sort/new";
        
        /// <summary>
        /// A subsection of content ordered by most popular to least popular.
        /// </summary>
        public static readonly string Popular = "http://opds-spec.org/sort/popular";
        
        /// <summary>
        /// A subsection of content that has been selected to be promoted to the client. 
        /// </summary>
        public static readonly string Featured = "http://opds-spec.org/featured";

        /// <summary>
        /// A subsection of content that has been selected to recommend to the client,
        /// the best matches to the specific client should be presented first.
        /// </summary>
        public static readonly string Recommended = "http://opds-spec.org/recommended";

        /// <summary>
        /// Indicates a relation to a crawlable resource (such as a paginated acquisition feed).
        /// </summary>
        public static readonly string Crawlable = "http://opds-spec.org/crawlable";

        /// <summary>
        /// Indicates a relation to a search feed.
        /// </summary>
        public static readonly string Search = "search";

        /// <summary>
        /// Indicates a relation to a faceted listing.
        /// </summary>
        public static readonly string Facet = "http://opds-spec.org/facet";

        /// <summary>
        /// An image associated with a catalog entry.
        /// </summary>
        public static readonly string Image = "http://opds-spec.org/image";
        
        /// <summary>
        /// A smaller image associated with a catalog entry.
        /// </summary>
        public static readonly string Thumbnail = "http://opds-spec.org/image/thumbnail";

        /// <summary>
        /// Supports a "crawlable" feed.  A relation to the current page.
        /// </summary>
        public static readonly string Self = "self";

        /// <summary>
        /// Supports a "crawlable" feed.  A relation to the first page.
        /// </summary>
        public static readonly string First = "first";
        
        /// <summary>
        /// Supports a "crawlable" feed.  A relation to the last page.
        /// </summary>
        public static readonly string Last = "last";
        
        /// <summary>
        /// Supports a "crawlable" feed.  A relation to the next page.
        /// </summary>
        public static readonly string Next = "next";
        
        /// <summary>
        /// Supports a "crawlable" feed.  A relation to the previous page.
        /// </summary>
        public static readonly string Previous = "previous";
    }
}