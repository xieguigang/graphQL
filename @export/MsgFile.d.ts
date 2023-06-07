// export R# package module type define for javascript/typescript language
//
//    imports "MsgFile" from "graphR";
//
// ref=graphR.MsgFile@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the graph database file I/O handler
 * 
*/
declare namespace MsgFile {
   /**
     * @param env default value Is ``null``.
   */
   function edgeSource(file: any, env?: object): any;
   /**
   */
   function getTerms(index: object): string;
   /**
    * open a message pack graph database file or 
    *  create a new empty graph database object.
    * 
    * 
     * @param file -
     * 
     * + default value Is ``null``.
     * @param evidenceAggregate 
     * + default value Is ``false``.
     * @param noGraph 
     * + default value Is ``false``.
     * @param seekIndex 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function open(file?: any, evidenceAggregate?: boolean, noGraph?: boolean, seekIndex?: boolean, env?: object): object|object;
   module read {
      /**
       * read target graph database as network graph object
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function graph(file: any, env?: object): object;
      /**
       * fetch the knowledge terms table from the graph database file.
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function knowledge_table(file: any, env?: object): object;
   }
   /**
    * save a graph database result into a file 
    *  in messagepack format.
    * 
    * 
     * @param kb -
     * @param file -
     * @param json_dump 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function save(kb: object, file: any, json_dump?: boolean, env?: object): any;
   /**
   */
   function seekTerm(index: object, term: string): any;
}
