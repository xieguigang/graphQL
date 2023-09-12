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
"A clear, odorless, tasteless liquid that is essential for most animal and plant life and is an excellent solvent for many substances. The chemical formula is hydrogen oxide (H2O). (McGraw-Hill Dictionary of Scientific and Technical Terms, 4th ed)",
 "Water is an inorganic compound with the chemical formula H2O. It is a transparent, tasteless, odorless,[c] and nearly colorless chemical substance, and it is the main constituent of Earth's hydrosphere and the fluids of all known living organisms (in which it acts as a solvent[19]). It is vital for all known forms of life, despite not providing food energy, or organic micronutrients. Its chemical formula, H2O, indicates that each of its molecules contains one oxygen and two hydrogen atoms, connected by covalent bonds. The hydrogen atoms are attached to the oxygen atom at an angle of 104.45°.[20] Water is also the name of the liquid state of H2O at standard temperature and pressure.",
    "Because Earth's environment is relatively close to water's triple point, water exists on Earth as a solid, liquid, and gas.[21] It forms precipitation in the form of rain and aerosols in the form of fog. Clouds consist of suspended droplets of water and ice, its solid state. When finely divided, crystalline ice may precipitate in the form of snow. The gaseous state of water is steam or water vapor.",
    "Water covers about 71% of the Earth's surface, with seas and oceans making up most of the water volume (about 96.5%).[22] Small portions of water occur as groundwater (1.7%), in the glaciers and the ice caps of Antarctica and Greenland (1.7%), and in the air as vapor, clouds (consisting of ice and liquid water suspended in air), and precipitation (0.001%).[23][24] Water moves continually through the water cycle of evaporation, transpiration (evapotranspiration), condensation, precipitation, and runoff, usually reaching the sea.",
    "Water plays an important role in the world economy. Approximately 70% of the freshwater used by humans goes to agriculture.[25] Fishing in salt and fresh water bodies has been, and continues to be, a major source of food for many parts of the world, providing 6.5% of global protein.[26] Much of the long-distance trade of commodities (such as oil, natural gas, and manufactured products) is transported by boats through seas, rivers, lakes, and canals. Large quantities of water, ice, and steam are used for cooling and heating in industry and homes. Water is an excellent solvent for a wide variety of substances, both mineral and organic; as such, it is widely used in industrial processes and in cooking and washing. Water, ice, and snow are also central to many sports and other forms of entertainment, such as swimming, pleasure boating, boat racing, surfing, sport fishing, diving, ice skating, and skiing."
];

print(text);

let g = text_graph(text);

for(par in g) {
    print(par);

    graphQL::push_graph(par);
}

# "chemicals" 2737272594