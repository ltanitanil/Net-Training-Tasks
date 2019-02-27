
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Serialization.Tasks
{
    //AIzaSyDHai9ckoRRODgF4XrhY-S-PaM_i8NbJp8
    // TODO: Implement GoogleSearchResult class to be deserialized from Google Search API response
    // Specification is available at: https://developers.google.com/custom-search/v1/using_rest#WorkingResults
    // The test json file is at Serialization.Tests\Resources\GoogleSearchJson.txt


    [DataContract(), KnownType(typeof(Url1)), KnownType(typeof(Queries)), KnownType(typeof(Item))]
    public class GoogleSearchResult
    {
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "url")]
        public Url1 Url { get; set; }
        [DataMember(Name = "queries")]
        public Queries Queries { get; set; }
        [DataMember(Name = "context")]
        public Context Context { get; set; }
        [DataMember(Name = "items")]
        public List<Item> Items { get; set; }


    }

    [DataContract]
    public class Url1
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "template")]
        public string Template { get; set; }
    }

    [DataContract,KnownType(typeof(Page))]
    public class Queries
    {
        [DataMember(Name = "nextPage")]
        public List<Page> NextPage { get; set; }
        [DataMember(Name = "previousPage")]
        public List<Page> PreviousPage { get; set; }
        [DataMember(Name = "request")]
        public List<Page> Request { get; set; }
    }

    [DataContract]
    public class Page
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "totalResults")]
        public long TotalResults { get; set; }
        [DataMember(Name = "searchTerms")]
        public string SearchTerms { get; set; }
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }
        [DataMember(Name = "inputEncoding")]
        public string InputEncoding { get; set; }
        [DataMember(Name = "outputEncoding")]
        public string OutputEncoding { get; set; }
        [DataMember(Name = "cx")]
        public string Cx { get; set; }
    }

    [DataContract]
    public class Context
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }

    [DataContract,KnownType(typeof(PageMap))]
    public class Item
    {
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "htmlTitle")]
        public string HtmlTitle { get; set; }
        [DataMember(Name = "link")]
        public string Link { get; set; }
        [DataMember(Name = "displayLink")]
        public string DisplayLink { get; set; }
        [DataMember(Name = "snippet")]
        public string Snippet { get; set; }
        [DataMember(Name = "htmlSnippet")]
        public string HtmlSnippet { get; set; }
        [DataMember(Name = "pagemap")]
        public PageMap PageMap { get; set; }
        
    }

    [DataContract, KnownType(typeof(RTO))]
    public class PageMap
    {
        [DataMember(Name = "RTO")]
        public List<RTO> RTO { get; set; }
    }
    [DataContract]
    public class RTO
    {
        [DataMember(Name = "format")]
        public string Format { get; set; }
        [DataMember(Name = "group_impression_tag")]
        public string Group_impression_tag { get; set; }
        [DataMember(Name = "Opt::max_rank_top")]
        public string Opt__max_rank_top { get; set; }
        [DataMember(Name = "Opt::threshold_override")]
        public string Opt__threshold_override { get; set; }
        [DataMember(Name = "Opt::disallow_same_domain")]
        public string Opt__disallow_same_domain { get; set; }
        [DataMember(Name = "Output::title")]
        public string Output__title { get; set; }
        [DataMember(Name = "Output::want_title_on_right")]
        public string Output__want_title_on_right { get; set; }
        [DataMember(Name = "Output::num_lines1")]
        public string Output__num_lines1 { get; set; }
        [DataMember(Name = "Output::text1")]
        public string Output__text1 { get; set; }
        [DataMember(Name = "Output::gray1b")]
        public string Output__gray1b { get; set; }
        [DataMember(Name = "Output::no_clip1b")]
        public string Output__no_clip1b { get; set; }
        [DataMember(Name = "UrlOutput::url2")]
        public string UrlOutput__url2 { get; set; }
        [DataMember(Name = "Output::link2")]
        public string Output__link2 { get; set; }
        [DataMember(Name = "Output::text2b")]
        public string Output__text2b { get; set; }
        [DataMember(Name = "UrlOutput::url2c")]
        public string UrlOutput__url2c { get; set; }
        [DataMember(Name = "Output::link2c")]
        public string Output__link2c { get; set; }
        [DataMember(Name = "result_group_header")]
        public string Result_group_header { get; set; }
        [DataMember(Name = "Output::image_url")]
        public string Output__image_url { get; set; }
        [DataMember(Name = "image_size")]
        public string Image_size { get; set; }
        [DataMember(Name = "Output::inline_image_width")]
        public string Output__inline_image_width { get; set; }
        [DataMember(Name = "Output::inline_image_height")]
        public string Output__inline_image_height { get; set; }
        [DataMember(Name = "Output::image_border")]
        public string Output__image_border { get; set; }
    }
}
