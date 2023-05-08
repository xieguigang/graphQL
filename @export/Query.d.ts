// export R# package module type define for javascript/typescript language
//
// ref=graphR.Query@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * graph database knowledge data query and insert
 * 
*/
declare namespace Query {
   /**
    * insert a knowledge node into the graph pool
    * 
    * 
     * @param knowledge -
     * @param meta -
     * 
     * + default value Is ``null``.
     * @param selfReference the database graph link can be build internal the identical database source
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function insert(kb:object, knowledge:string, type:string, meta?:object, selfReference?:boolean, env?:object): any;
   module ignore {
      /**
       * ignores the given data types when build graph links.
       * 
       * 
        * @param kb -
        * @param ignores -
      */
      function evidenceLink(kb:object, ignores:any): object;
   }
   /**
   */
   function join(kb1:object, kb2:object): object;
   /**
    * query knowledge data for a given term
    * 
    * 
     * @param kb -
     * @param term -
     * @param cutoff -
     * 
     * + default value Is ``0``.
     * @param env 
     * + default value Is ``null``.
   */
   function query(kb:object, term:string, cutoff?:number, env?:object): object;
   /**
    * measure the similarity or identical between two 
    *  knowledge terms based on the knowledge network 
    *  that we've build.
    * 
    * 
     * @param kb -
     * @param x -
     * @param y -
     * @param weight -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function similarity(kb:object, x:string, y:string, weight?:object, env?:object): number;
}
