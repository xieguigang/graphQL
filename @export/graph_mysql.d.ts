// export R# package module type define for javascript/typescript language
//
//    imports "graph_mysql" from "graphR";
//
// ref=graphR.graphMySQLTool@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace graph_mysql {
   /**
     * @param category default value Is ``'unclass'``.
     * @param metadata default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function add_term(graphdb: object, term: string, category?: string, metadata?: object, env?: object): any;
   module open {
      /**
        * @param host default value Is ``'localhost'``.
        * @param port default value Is ``3306``.
        * @param dbname default value Is ``'graphql'``.
      */
      function graphdb(user_name: string, password: string, host?: string, port?: object, dbname?: string): object;
   }
}
