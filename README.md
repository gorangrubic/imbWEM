# imbWEM - Web Exploration Model

Web Crawling framework, for .NET Framework 4.0 (developed in C#) with multilingual support and wide range of customization options.


Learn more: 
http://blog.veles.rs

From article covering use of imbWEM:

> The Web Exploration Model (WEM) defines the execution flow with Stage Control Model, which is an ordinal sequence of Stage Model instances. By convention Stage Model instance currently hosting the crawler is simply called the Stage. The WEM is high abstraction model, supporting wide range of crawl operations that overcome the scope of this paper, therefore, only relevant aspects are described here. In this respect, we consider implementation with single instance of the Stage Model, having one or few Objective(s) associated with it. The Objective used in the evaluation experiments, uses static criteria stating the crawl volume limits, leading to the termination of the crawl process once the limits are met. Just as illustration of possible roles the class may have: for the preliminary survey the Objective was to detect all Targets within the domain – using a dynamic criterion. Beside the Objective, the Target and the Page instances are subject of evaluation by the Frontier Modules driving the crawl.
