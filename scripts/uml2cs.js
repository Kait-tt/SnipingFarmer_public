// Node.js > v8.1.2

const fs = require('fs');
const assert = require('assert');

const USAGE = `UMLをUnityScriptの変換する。
継承や実装は変換しない。

  Usage: node uml2cs.js <source> <dist>
    - source : Source PlantUML file
    - dist : Distribution directory
  
  Example:
    node uml2cs.js documents/class/game.pml _gen
`;

const ROOT_NS = ['SnipingFarmer', 'Script'];

if (process.argv.length < 4) {
    console.error(USAGE);
    process.exit(1);
}

const relationWords = ['>', '<', '*', 'o', '|>', '<|']
    .map(x => ['.' + x, '-' + x])
    .reduce((ys, xs) => {
        xs.forEach(x => {
            ys.push(x);
            ys.push(x.split('').reverse().join(''));
        });
        return ys;
    }, []);

const pmlPath = process.argv[2];
const pmlDir = (() => {
    const ps = pmlPath.split(/[\\|\/]/);
    ps.pop();
    return ps.join('/');
})();
const distDir = process.argv[3];

const lines = load(pmlPath);

let idx = 0;
const files = [];
parse();
genFiles();

function load (path) {
    console.log(`load: ${path}`);
    if (!fs.existsSync(path)) {
        throw new Error(`${path} is not found`);
    }

    const pml = fs.readFileSync(path, 'utf-8');
    const lines = pmlToLines(pml);

    // include
    return lines.reduce((ys, line) => {
        if (line.startsWith('!include')) {
            const filename = line.split(' ')[1];
            const npath = pmlDir + '/'+ filename;
            const nlines = load(npath);
            ys.push.apply(ys, nlines);
        } else {
            ys.push(line);
        }
        return ys;
    }, []);
}

function pmlToLines (pml) {
    return pml
        .split(/\n/)
        .map(x => x.trim())
        .filter(x => x)
        // ignore meta and relation
        .filter(x => {
            return !(
                x.startsWith('@') ||
                relationWords.some(y => x.includes(y))
            );
        });
}

function parse () {
    while (idx < lines.length) {
        if (lines[idx].startsWith('namespace')) {
            parseNamespace([...ROOT_NS]);
        }
    }
}

function parseNamespace (ns) {
    const ws = toWords(lines[idx]);
    ns.push(ws[1]);

    ++idx;

    while (lines[idx] !== '}') {
        if (lines[idx].startsWith('namespace')) {
            parseNamespace([...ns]);
        } else if (['class', 'interface', 'abstract'].some(k => lines[idx].startsWith(k))) {
            parseClass([...ns]);
        } else if (lines[idx].startsWith('enum')) {
            parseEnum([...ns]);
        } else {
            throw new Error(`Invalid line: ${lines[idx]}`);
        }
    }

    ++idx;
}

function parseClass (ns) {
    const ws = toWords(lines[idx]);
    const isAbstract = ws[0] === 'abstract';
    const isInterface = ws[0] === 'interface';
    const name = ws[1];

    assert(!lines[idx].includes('static'));

    const raw = [];
    raw.push(lines[idx]);

    let members = [];

    if (lines[idx].includes('{')) {
        ++idx;
        let isSerializable = false;
        while (lines[idx] !== '}') {
            const ws = toWords(lines[idx]);
            if (lines[idx] === '.. serializable ..') {
                isSerializable = true;
            } else if (lines[idx] === '....') {
                isSerializable = false;
            } else if (ws[0] === '____') {
                // skip
            } else {
                // is member
                members.push(parseClassMember(lines[idx], isSerializable));
            }

            raw.push(lines[idx]);
            ++idx;
        }

        assert(!isSerializable);
    } else {
    }

    raw.push(lines[idx]);

    files.push({
        type: 'class',
        isAbstract,
        isInterface,
        ns,
        name,
        members,
        raw
    });

    ++idx;
}

function parseClassMember (line, isSerializable) {
    const res = {
        type: null,
        name: null,
        access: null,
        isStatic: null,
        isAbstract: null,
        isSerializable,
        valType: null,
        args: null
    };

    const ws = toWords(line);
    let wi = 0;

    if (ws[wi] === '~') {
        res.access = 'protected';
    } else if (ws[wi] === '-') {
        res.access = 'private';
    } else if (ws[wi] === '+') {
        res.access = 'public';
    } else {
        throw new Error(`Invalid access modifier: ${ws[wi]}`);
    }
    assert(res.access);
    ++wi;

    while (wi < ws.length) {
        if (ws[wi] === '{static}') {
            res.isStatic = true;
        } else if (ws[wi] === '{abstract}') {
            res.isAbstract = true;
        } else {
            break;
        }
        ++wi;
    }

    if (line.includes('(')) {
        res.type = 'method';

        res.valType = ws[wi];
        ++wi;

        res.name = ws[wi];
        assert(!res.name.includes('('), line);
        ++wi;

        const s = wi;
        const e = ws.indexOf(ws.find(w => w.endsWith(')')), s);
        assert(e !== -1);

        if (s === e) {
            res.args = [];
        } else {
            res.args = [ws[s].substr(1), ...ws.slice(s + 1, e), ws[e].substr(0, ws[e].length - 1)]
                .map(x => x.trim())
                .filter(x => x);
        }
        wi = e + 1;
    } else {
        res.valType = ws[wi];
        ++wi;

        res.name = ws[wi];
        ++wi;

        if (/[a-z]/.test(res.name.substr(0, 1))) {
            res.type = 'member';
        } else {
            res.type = 'property';
        }
    }

    assert(wi === ws.length, `'${line}' , '${ws[wi]}'`);

    return res;
}

function parseEnum (ns) {
    const ws = toWords(lines[idx]);
    const name = ws[1];

    const raw = [];
    raw.push(lines[idx]);

    const members = [];

    if (lines[idx].includes('{')) {
        ++idx;
        while (lines[idx] !== '}') {
            members.push(lines[idx]);
            raw.push(lines[idx]);
            ++idx;
        }
    } else {
    }

    raw.push(lines[idx]);

    files.push({
        type: 'enum',
        ns,
        name,
        members,
        raw
    });

    ++idx;
}

function toWords (line) {
    return line
        .split(' ')
        .map(x => x.trim())
        .filter(x => x);
}

function genFiles () {
    files.forEach(data => {
        if (data.type === 'class') {
            data.code = serializeClass(data);
        } else if (data.type === 'enum') {
            data.code = serializeEnum(data);
        } else {
            throw new Error(`Invalid type: ${data.type}`);
        }
    });

    files.forEach(({code, ns, name}) => {
        const dir = `${distDir}/${ns.join('/')}`;
        const path = `${dir}/${name}.cs`;
        console.log(`generate: ${path}`);
        mkdirp(dir);
        fs.writeFileSync(path, code, 'utf-8');
    });

}

function serializeClass ({ns, name, isAbstract, isInterface, members, row}) {
    const typeStr = (() => {
        if (isAbstract) { return 'abstract class'; }
        else if (isInterface) { return 'interface'; }
        else return 'class';
    })();

    assert(!members.filter(({type}) => !['member', 'property', 'method'].includes(type)).length);

    const memberLines = [
        ...members
            .filter(({type}) => type === 'member')
            .map(({name, access, isStatic, isAbstract, isSerializable, valType, args}) => {
                assert(name);
                assert(access);
                assert(access !== 'protected');
                assert(valType);
                assert(isStatic === null);
                assert(isAbstract === null);
                assert(args === null);

                let mLines = [];

                if (isSerializable) {
                    mLines.push('[SerializeField]');
                }

                mLines.push(`${access} ${valType} ${name};`);

                return mLines;
            }),
        // properties
        ...members
            .filter(({type}) => type === 'property')
            .map(({name, access, isStatic, isAbstract, isSerializable, valType, args}) => {
                assert(name);
                assert(access);
                assert(valType);
                assert(isStatic === null);
                assert(isAbstract === null);
                assert(args === null);

                let mLines = [];

                if (isSerializable) {
                    mLines.push('[SerializeField]');
                }

                if (isInterface) {
                    mLines.push(`${valType} ${name} { get; set; }`);
                } else if (access === 'protected') {
                    mLines.push(`public ${valType} ${name} { get; private set; }`);
                } else {
                    mLines.push(`${access} ${valType} ${name} { get; set; }`);
                }

                return mLines;
            }),
        // methods
        ...members
            .filter(({type}) => type === 'method')
            .map(({name, access, isStatic, isAbstract, isSerializable, valType, args}) => {
                assert(name);
                assert(access);
                assert(valType);
                assert(!isSerializable);
                assert(isAbstract === null);
                assert(args);

                let mLines = [];

                assert(!(args.length % 2));
                const argsWords = [];
                for (let i = 0; i < args.length; i += 2) {
                    argsWords.push(`${args[i]} ${args[i + 1]}`);
                }
                const argsStr = argsWords.join(' ');

                let str;
                if (isInterface) {
                    str = `${valType} ${name}(${argsStr})`;
                } else {
                    str = `${access} ${valType} ${name}(${argsStr})`;
                }

                if (isStatic) {
                    str = `static ${str}`;
                }


                if (isInterface) {
                    str += ';';
                    mLines.push(str);
                } else {
                    mLines.push(str);
                    mLines.push('{');
                    mLines.push('    // TODO');
                    mLines.push('}');
                }

                return mLines;
            })
    ].reduce((y, xs) => {
        xs.forEach(x => {
            y.push(x);
        });
        y.push('');
        return y;
    }, []);
    memberLines.pop();

    const implementMonoStr = isInterface ? '' : ' : MonoBehaviour';

    return [
        `using UnityEngine;`,
        ``,
        `namespace ${ns.join('.')}`,
        `{`,
        `    ${typeStr} ${name}${implementMonoStr}`,
        `    {`,
        ...memberLines.map(x => `        ${x}`).map(x => x.trim() ? x : ''),
        `    }`,
        `}`
    ].join('\n');
}

function serializeEnum ({ns, name, members}) {
    return [
        `namespace ${ns.join('.')}`,
        `{`,
        `    enum ${name}`,
        `    {`,
        members.map(member => {
            return `        ${member}`;
        }).join(',\n'),
        `    }`,
        `}`
    ].join('\n');
}

function mkdirp (path) {
    const ps = [];
    path.split('/').forEach(dir => {
        ps.push(dir);
        const p = ps.join('/');
        if (!fs.existsSync(p)) {
            console.log(`mkdir ${p}`);
            fs.mkdirSync(p);
        }
    });
}
