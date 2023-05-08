// export R# package module type define for javascript/typescript language
//
// ref=graphR.KnowledgeGraph@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace KnowledgeGraph {
   /**
   */
   function removeLinkByWeight(g:object, w:number): object;
   /**
    * export the graph database as the 
    *  network graph model for run 
    *  algorithm debug.
    * 
    * 
     * @param kb -
     * @param filters 
     * + default value Is ``null``.
     * @return nodes meta: knowledge_type
   */
   function networkGraph(kb:object, filters?:any): object;
   module Kosaraju {
      /**
      */
      function SCCs(g:object): object;
   }
   /**
     * @param eps default value Is ``0.1``.
   */
   function graphUMAP(g:object, eps?:number): any;
   /**
     * @param weightCut default value Is ``-1``.
     * @param identicalKeys default value Is ``null``.
   */
   function knowledgeIslands(graph:object, weightCut?:number, identicalKeys?:string): object;
   /**
     * @param equals default value Is ``0.5``.
   */
   function extractKnowledgeTerms(island:object, equals?:number): object;
   module as {
      /**
       * direct cast the graph object as a knowledge term
       * 
       * 
        * @param island -
      */
      function knowledge(island:object): object;
   }
   /**
    * make meta data unique at first and then evaluate 
    *  the unique reference id via FNV-1a hash function
    * 
    * > this function has a special rule for the knowledge 
    * >  term its ``name`` field.
    * 
     * @param knowledges -
     * @param kb -
     * @param indexBy -
     * 
     * + default value Is ``null``.
     * @param prefix -
     * 
     * + default value Is ``'Term'``.
     * @param width -
     * 
     * + default value Is ``10``.
   */
   function niceTerms(knowledges:object, kb:object, indexBy?:any, prefix?:string, width?:object): object;
   /**
   */
   function correctKnowledges(kb:object, knowledges:object, indexBy:any): object;
   /**
    * export knowledge terms based on the network community algorithm
    * 
    * 
     * @param kb -
     * @param common_type all of the type defined from this parameter will be removed from 
     *  the community algorithm due to the reason of common type always 
     *  be a hub node in the network, will create a false knowledge community 
     *  result. example as formula string in chemical data knowledges 
     *  will groups all Isomer compounds with the same formula string as 
     *  one identical metabolite.
     * 
     * + default value Is ``null``.
     * @param eps 
     * + default value Is ``0.001``.
     * @param unweighted 
     * + default value Is ``false``.
     * @return this function returns a tuple list with two elements inside:
     *  
     *  1. ``graph`` - is the knowledge network graph data with community 
     *                 tags and trimmed data.
     *  2. ``knowledges`` - a table dataset that contains knowledge data 
     *                      entities that detects from the network graph 
     *                      community data result.
   */
   function knowledgeCommunity(kb:object, indexBy:any, common_type?:any, eps?:number, unweighted?:boolean): object;
}
