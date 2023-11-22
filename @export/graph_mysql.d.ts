// export R# package module type define for javascript/typescript language
//
//    imports "graph_mysql" from "graphR";
//
// ref=graphR.graphMySQL@graphR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace graph_mysql {
   module open {
      /**
        * @param host default value Is ``'localhost'``.
        * @param port default value Is ``3306``.
        * @param dbname default value Is ``'graphql'``.
      */
      function graphdb(user_name: string, password: string, host?: string, port?: object, dbname?: string): object;
   }
}
