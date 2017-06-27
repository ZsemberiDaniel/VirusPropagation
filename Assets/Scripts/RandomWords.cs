﻿
public class RandomWords {
    private static string[] randomWords = new string[] {
        "abandon", "abbreviate", "abstract", "academic", "access", "accommodation", "accompanied", "according", "accumulation",
        "accurate", "achieve", "acknowledged", "acquisition", "acronym", "adaptation", "address", "adequate", "adjacent",
        "adjustment", "administration", "adults", "advocate", "affect", "aggregate", "aid", "albeit", "allocation", "alter",
        "alternative", "always", "ambiguous", "amendment", "analogous", "analogy", "analyse", "analysis", "analyze", "annotate",
        "annual", "anticipate", "anticipated", "apparent", "appendix", "application", "apply", "appreciation", "approach",
        "appropriate", "approximate", "approximated", "arbitrary", "area", "argue", "argument", "arrange", "articulate",
        "aspects", "assemble", "assembly", "assert", "assess", "assessment", "assigned", "assistance", "associate", "assume",
        "assumption", "assurance", "attached", "attained", "attitudes", "attributed", "audience", "authentic", "author",
        "authority", "automatically", "available", "aware", "background", "behalf", "benefit", "bias", "body", "bond",
        "brainstorm", "brief", "bulk", "Burke", "calculate", "capable", "capacity", "caption", "categories", "category", "cause",
        "ceases", "challenge", "channel", "chapter", "character", "characteristic", "characterize", "chart", "chemical",
        "chronology", "circumstances", "citation", "cite", "cited", "civil", "claim", "clarify", "class", "classical",
        "classroom", "clause", "clue", "code", "coherence", "coherent", "coincide", "collapse", "colleagues", "commenced",
        "comments", "commission", "commitment", "commodity", "common", "communication", "community", "compare", "compensation",
        "compile", "compiled", "complement", "complete", "complex", "components", "compose", "composition", "compounds",
        "comprehensive", "comprise", "computer", "conceive", "conceived", "concentration", "concept", "concise", "conclude",
        "conclusion", "concrete", "concurrent", "conditions", "conduct", "conference", "confined", "confirm", "confirmed",
        "conflict", "conformity", "consent", "consequence", "consequences", "consider", "considerable", "consist", "consistent",
        "consistently", "constant", "constitutes", "constitutional", "constraints", "construction", "consult", "consultation",
        "consumer", "contact", "contemporary", "contend", "context", "continuum", "contract", "contradict", "contradiction",
        "contrary", "contrast", "contribution", "control", "controversy", "convention", "conversely", "convert", "converted",
        "convey", "convinced", "cooperative", "coordination", "copy", "core", "corporate", "correlate", "correspond",
        "corresponding", "couple", "create", "credible", "credit", "criteria", "critique", "crucial", "cultural", "cumulative",
        "currency", "cycle", "data", "debate", "decades", "decline", "deduce", "deduction", "defend", "define", "definite",
        "definition", "demand", "demonstrate", "denote", "deny", "depict", "depression", "derive", "derived", "describe",
        "design", "despite", "detail", "detect", "detected", "determine", "develop", "deviation", "device", "devise", "devoted",
        "diction", "differentiate", "differentiation", "dimension", "dimensions", "diminish", "diminished", "direct",
        "discipline", "discover", "discretion", "discriminate", "discrimination", "discuss", "displacement", "display",
        "disposal", "distinction", "distinguish", "distorted", "distortion", "distribution", "diversity", "document", "domain",
        "domestic", "dominant", "draft", "dramatic", "draw", "duration", "dynamic", "economic", "edit", "edition", "effect",
        "elements", "eliminate", "emerged", "emphasis", "emphasize", "empirical", "employ", "enable", "encountered", "energy",
        "enforcement", "enhanced", "enormous", "ensure", "entities", "environment", "equal", "equation", "equipment",
        "equivalent", "erosion", "error", "essay", "essential", "establish", "established", "estate", "estimate", "ethical",
        "ethnic", "evaluate", "evaluation", "event", "eventually", "evidence", "evolution", "exaggerate", "examine", "example",
        "exceed", "excerpt", "exclude", "excluded", "exercise", "exhibit", "expansion", "expert", "explain", "explicit",
        "exploitation", "explore", "export", "expository", "exposure", "external", "extract", "facilitate", "fact", "factor",
        "factors", "feature", "features", "federal", "fees", "figurative", "figure", "file", "final", "financial", "finite",
        "flexibility", "fluctuations", "focus", "footer", "for", "foreshadow", "form", "format", "former", "formula", "formulate",
        "forthcoming", "foundation", "founded", "fragment", "frame", "framework", "frequently", "function", "fundamental",
        "funds", "furthermore", "gender", "general", "generated", "generation", "genre", "global", "goals", "grade", "granted",
        "graph", "graphic", "guarantee", "guidelines", "header", "heading", "hence", "hierarchical", "highlight", "highlighted",
        "hypothesis", "hypothesise", "hypothesize", "identical", "identified", "identify", "ideology", "ignored", "illustrate",
        "illustrated", "image", "imitate", "immigration", "impact", "implementation", "implications", "implicit", "implies",
        "imply", "imposed", "incentive", "incidence", "inclination", "inclined", "include", "income", "incompatible",
        "incorporate", "incorporated", "index", "indicate", "indirect", "individual", "induced", "inevitably", "infer",
        "inferred", "influence", "inform", "information", "infrastructure", "inherent", "inhibition", "initial", "initiatives",
        "injury", "innovation", "input", "inquire", "insert", "insights", "inspection", "instance", "institute", "instructions",
        "integral", "integrate", "integration", "integrity", "intelligence", "intensity", "intent", "intention", "interact",
        "interaction", "intermediate", "intermittent", "internal", "interpret", "interpretation", "interval", "intervention",
        "intrinsic", "introduce", "introduction", "invariably", "investigate", "investigation", "investment", "invoked",
        "involve", "involved", "irony", "irrelevant", "isolate", "isolated", "issues", "italics", "items", "job", "journal",
        "judge", "justification", "key", "label", "labour", "layer", "lecture", "legal", "legislation", "levy", "liberal",
        "licence", "likely", "likewise", "link", "list", "literal", "locate", "location", "logic", "logical", "main",
        "maintenance", "major", "manipulation", "manual", "margin", "marginal", "mature", "maximum", "may", "mean", "measure",
        "mechanism", "media", "mediation", "medical", "medium", "mental", "metaphor", "method", "migration", "military",
        "minimal", "minimised", "minimum", "ministry", "minorities", "mode", "model", "modified", "modify", "monitor",
        "monitoring", "more", "motivation", "mutual", "narrative", "narrator", "negative", "network", "neutral", "never",
        "nevertheless", "nonetheless", "normal", "norms", "notation", "note", "notice", "notion", "notwithstanding", "nuclear",
        "objective", "observe", "obtained", "obvious", "occupational", "occur", "odd", "offset", "ongoing", "opinion", "oppose",
        "option", "optional", "order", "organize", "orientation", "origins", "outcomes", "outline", "output", "overall",
        "overlap", "overseas", "pace", "panel", "paradigm", "paragraph", "parallel", "parameters", "paraphrase", "participation",
        "partnership", "passage", "passive", "pattern", "perceived", "percent", "perform", "period", "persistent", "perspective",
        "persuade", "phase", "phenomenon", "philosophy", "physical", "place", "plagiarism", "plan", "plausible", "plot",
        "plus", "point", "policy", "portion", "portray", "posed", "positive", "possible", "potential", "practitioners",
        "preceding", "precise", "preclude", "predict", "predicted", "predominantly", "prefix", "preliminary", "prepare",
        "presume", "presumption", "preview", "previous", "primary", "prime", "principal", "principle", "prior", "priority",
        "probable", "procedure", "process", "produce", "professional", "profile", "prohibited", "project", "promote", "prompt",
        "proofread", "property", "proportion", "propose", "prose", "prospect", "protocol", "prove", "psychology", "publication",
        "published", "purchase", "purpose", "pursue", "qualitative", "quotation", "quote", "radical", "random", "range", "rank",
        "rare", "rarely", "ratio", "rational", "reaction", "recall", "recovery", "reduce", "refer", "refine", "reflect", "regime",
        "region", "registered", "regular", "regulations", "reinforced", "rejected", "relate", "relationship", "relaxed",
        "release", "relevant", "reliance", "reluctant", "removed", "rephrase", "report", "represent", "request", "require",
        "required", "requisite", "research", "resident", "resolution", "resources", "respond", "response", "responsible",
        "restate", "restore", "restraints", "restricted", "results", "retained", "reveal", "revealed", "revenue", "reverse",
        "review", "revise", "revision", "revolution", "rigid", "role", "root", "route", "rule", "scan", "scenario", "schedule",
        "scheme", "scope", "score", "section", "sector", "security", "select", "sequence", "series", "set", "setting", "sex",
        "shift", "show", "signal", "significance", "significant", "similar", "simile", "simulation", "site", "skim", "so called",
        "solely", "solve", "somewhat", "sought", "source", "spatial", "specific", "specified", "speculate", "sphere",
        "stability", "stance", "standard", "state", "statement", "statistics", "status", "straightforward", "strategies",
        "strategy", "stress", "structure", "study", "style", "styles", "subject", "subjective", "submitted", "subordinate",
        "subsequent", "subsidiary", "substitute", "substitution", "successive", "succinct", "sufficient", "suggest", "sum",
        "summarize", "summary", "supplementary", "support", "survey", "survive", "suspended", "sustainable", "symbolic",
        "symbolize", "synonym", "synthesize", "table", "tapes", "target", "task", "team", "technical", "technique", "techniques",
        "technology", "temporary", "tension", "term", "termination", "test", "text", "theme", "theory", "thereby", "thesis",
        "timeline", "tone", "topic", "trace", "traditional", "trait", "transfer", "transformation", "transition", "translate",
        "transmission", "transporttrend", "trigger", "typically", "ultimately", "undergo", "underlying", "undertaken", "unified",
        "uniform", "unique", "use", "utilise", "utility", "utilize", "valid", "validity", "variables", "variation", "vary",
        "vehicle", "verify", "version", "via", "view", "viewpoint", "violation", "virtually", "visible", "vision", "Visit",
        "visual", "voice", "volume", "voluntary", "welfare", "whereas", "whereby"
    };
    private static System.Random rnd = new System.Random();

    public static string getRandomWords(int count = 1) {
        string words = "";
        string currWord;
        for (int i = 0; i < count; i++) {
            currWord = randomWords[rnd.Next(0, randomWords.Length)];
            words += currWord.Substring(0, 1).ToUpper() + currWord.Substring(1, currWord.Length - 1);
        }

        return words;
    }

}