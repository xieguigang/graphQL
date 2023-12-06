// export R# package module type define for javascript/typescript language
//
//    imports "graph_mysql" from "graphR";
//
// ref=graphR.graphMySQLTool@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace graph_mysql {
   /**
     * @param category default value Is ``'unclass'``.
     * @param metadata default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function add_term(graphdb: object, term: string, category?: string, metadata?: object, env?: object): any;
   module as {
      /**
       * cast the mysql object as the knowledge builder object
       * 
       * 
        * @param graphdb -
        * @param signature -
        * 
        * + default value Is ``null``.
      */
      function knowledge_builder(graphdb: object, signature?: object): object;
   }
   /**
    * assign the knowledge term id to the knowledge nodes
    * 
    * 
     * @param graphdb -
     * @param g -
     * @param term -
   */
   function assign_graph(graphdb: object, g: object, term: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function fetch_json(graphdb: any, id: string, env?: object): any;
   /**
     * @param row_builder default value Is ``null``.
     * @param n default value Is ``100000``.
     * @param env default value Is ``null``.
   */
   function fetch_table(graphdb: any, headers: any, row_builder?: object, n?: object, env?: object): any;
   module open {
      /**
        * @param host default value Is ``'localhost'``.
        * @param port default value Is ``3306``.
        * @param dbname default value Is ``'graphql'``.
      */
      function graphdb(user_name: string, password: string, host?: string, port?: object, dbname?: string): object;
   }
   /**
    * pull a knowledge graph
    * 
    * 
     * @param graphdb -
     * @param vocabulary -
     * @param id used for debug test
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pull_nextGraph(graphdb: object, vocabulary: any, id?: object, env?: object): object;
   module save {
      /**
       * save the knowledge data json into database as cache
       * 
       * 
        * @param graphdb -
        * @param seed -
        * @param term -
        * @param unique_hash the slot key name for get the combination term for generates the hashcode
        * @param knowledge -
        * @param env 
        * + default value Is ``null``.
      */
      function knowledge(graphdb: object, seed: object, term: string, unique_hash: string, knowledge: object, env?: object): any;
   }
}
