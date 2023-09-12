require(graphQL);
require(JSON);

let text = [
    "Water appears as a clear, nontoxic liquid composed of hydrogen and oxygen, essential for life and the most widely used solvent. Include water in a mixture to learn how it could react with other chemicals in the mixture.",
"Water is an oxygen hydride consisting of an oxygen atom that is covalently bonded to two hydrogen atoms It has a role as an amphiprotic solvent, a member of greenhouse gas, a human metabolite, a Saccharomyces cerevisiae metabolite, an Escherichia coli metabolite and a mouse metabolite. It is an oxygen hydride, a mononuclear parent hydride and an inorganic hydroxy compound. It is a conjugate base of an oxonium. It is a conjugate acid of a hydroxide.",
"	
Water appears as a clear, nontoxic liquid composed of hydrogen and oxygen, essential for life and the most widely used solvent. Include water in a mixture to learn how it could react with other chemicals in the mixture.
CAMEO Chemicals
Water is an oxygen hydride consisting of an oxygen atom that is covalently bonded to two hydrogen atoms It has a role as an amphiprotic solvent, a member of greenhouse gas, a human metabolite, a Saccharomyces cerevisiae metabolite, an Escherichia coli metabolite and a mouse metabolite. It is an oxygen hydride, a mononuclear parent hydride and an inorganic hydroxy compound. It is a conjugate base of an oxonium. It is a conjugate acid of a hydroxide.
ChEBI
Water (chemical formula: H2O) is a transparent fluid which forms the world's streams, lakes, oceans and rain, and is the major constituent of the fluids of organisms. As a chemical compound, a water molecule contains one oxygen and two hydrogen atoms that are connected by covalent bonds. Water is a liquid at standard ambient temperature and pressure, but it often co-exists on Earth with its solid state, ice; and gaseous state, steam (water vapor).",
"Water is a metabolite found in or produced by Escherichia coli (strain K12, MG1655).",
"Water is h2O, a clear, colorless, odorless, tasteless liquid that freezes into ice below 0 degrees centigrade and boils above 100 degrees centigrade.",
"A clear, odorless, tasteless liquid that is essential for most animal and plant life and is an excellent solvent for many substances. The chemical formula is hydrogen oxide (H2O). (McGraw-Hill Dictionary of Scientific and Technical Terms, 4th ed)"
];
let g = text_graph(text);

for(par in g) {
    print(par);

    graphQL::push_graph(par);
}

# "chemicals" 2737272594