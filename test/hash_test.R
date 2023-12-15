canonical_formula = "C6H13NO2";

kegg = ["L-Leucine"
"2-Amino-4-methylvaleric acid"
"(2S)-alpha-2-Amino-4-methylvaleric acid" "(2S)-alpha-Leucine" "leucine"];
hmdb = [
    "(2S)-2-Amino-4-methylpentanoic acid" "(2S)-alpha-2-Amino-4-methylvaleric acid" "(2S)-alpha-Leucine"
"(S)-(+)-Leucine" "(S)-Leucine" "2-Amino-4-methylvaleric acid" "L-Leucin" "L-Leuzin" "Leu" "LEUCINE"
"(2S)-2-Amino-4-methylpentanoate" "(2S)-a-2-Amino-4-methylvalerate" "(2S)-a-2-Amino-4-methylvaleric acid"
"(2S)-alpha-2-Amino-4-methylvalerate" "(2S)-Α-2-amino-4-methylvalerate" "(2S)-Α-2-amino-4-methylvaleric acid"
"(2S)-a-Leucine" "(2S)-Α-leucine" "2-Amino-4-methylvalerate" "(S)-2-Amino-4-methylpentanoate"
"(S)-2-Amino-4-methylpentanoic acid" "(S)-2-Amino-4-methylvalerate" "(S)-2-Amino-4-methylvaleric acid"
"4-Methyl-L-norvaline" "L-(+)-Leucine" "L-a-Aminoisocaproate" "L-a-Aminoisocaproic acid"
"L-alpha-Aminoisocaproate" "L-alpha-Aminoisocaproic acid" "Leucine, L-isomer" "L-Isomer leucine"
"Leucine, L isomer"
];
chebi = ["leucine" "(+-)-Leucine" "(RS)-Leucine" "2-amino-4-methylpentanoic acid"
"DL-Leucine" "Hleu" "Leu"	"Leucin" "Leuzin"];

kegg_fnv1a  = sapply(`${tolower(kegg)}+${canonical_formula}`,  si -> FNV1a(si));
hmdb_fnv1a  = sapply(`${tolower(hmdb)}+${canonical_formula}`,  si -> FNV1a(si));
chebi_fnv1a = sapply(`${tolower(chebi)}+${canonical_formula}`, si -> FNV1a(si));

print("kegg index:");
print(kegg_fnv1a);
print("hmdb index:");
print(hmdb_fnv1a);
print("chebi index:");
print(chebi_fnv1a);

const overlap = function(x) {
    x |> groupBy() 
      |> which(id -> length(id) > 1)
      |> orderBy(id -> length(id), desc = TRUE) 
      |> lapply(a -> length(a), names = a -> `BioDeep_${str_pad(a$key, 11, pad = "0")}`)
      ;
}

biodeep_id = overlap([...chebi_fnv1a, ...hmdb_fnv1a, ...kegg_fnv1a]); 

print("biodeep id based on the hash value:");        
str(biodeep_id);
print("primary id:");
print(first(names(biodeep_id)));

print(`chebi overlap with kegg: ${length(overlap([...chebi_fnv1a, ...kegg_fnv1a]))}`);
print(`hmdb overlap with kegg: ${length(overlap([...hmdb_fnv1a, ...kegg_fnv1a]))}`);
print(`chebi overlap with hmdb: ${length(overlap([...chebi_fnv1a, ...hmdb_fnv1a]))}`);