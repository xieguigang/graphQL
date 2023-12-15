// export R# source type define for javascript/typescript language
//
// package_source=graphQL

declare namespace graphQL {
   module _ {
      /**
      */
      function onLoad(): object;
   }
   /**
   */
   function __cosine(va: any, vb: any): object;
   /**
   */
   function __jaccard(va: any, vb: any): object;
   /**
     * @param top default value Is ``10``.
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function context_cosine(a: any, b: any, top?: any, graphdb?: any): object;
   def_ignores: any;
   /**
     * @param word_size default value Is ``6``.
     * @param top default value Is ``30``.
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function definition(word: any, word_size?: any, top?: any, graphdb?: any): object;
   /**
     * @param top default value Is ``10``.
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function get_prompt(word: any, top?: any, graphdb?: any): object;
   /**
     * @param w default value Is ``1``.
   */
   function graph_link(from: any, to: any, w?: any): object;
   /**
   */
   function graph_tokens(graph_df: any): object;
   /**
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function import_stopwords(graphdb?: any): object;
   /**
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function import_verbs(graphdb?: any): object;
   /**
     * @param phase_size default value Is ``3``.
   */
   function one_graph(tokens: any, phase_size?: any): object;
   /**
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function push_graph(graph_df: any, graphdb?: any): object;
   /**
     * @param phase_size default value Is ``6``.
   */
   function text_graph(text: any, phase_size?: any): object;
   /**
     * @param top default value Is ``10``.
     * @param graphdb default value Is ``Call "getOption"("graphdb_web")``.
   */
   function token_vector(token: any, top?: any, graphdb?: any): object;
   /**
   */
   function tokens_trim(tokens: any): object;
}
