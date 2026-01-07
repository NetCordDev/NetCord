// Type declarations for external modules without official @types packages

declare module 'lit-html' {
  export interface TemplateResult {
    strings: TemplateStringsArray
    values: readonly unknown[]
  }
  export const nothing: symbol
  export function html(strings: TemplateStringsArray, ...values: unknown[]): TemplateResult
  export function render(result: TemplateResult, container: HTMLElement | DocumentFragment): void
}

declare module 'lit-html/directives/class-map.js' {
  export function classMap(classInfo: Record<string, boolean | undefined>): unknown
}

declare module 'highlight.js' {
  export interface HighlightResult {
    value: string
    language?: string
    relevance: number
  }
  export function highlightElement(element: HTMLElement): void
  export function highlight(code: string, options: { language: string }): HighlightResult
  export function highlightAuto(code: string): HighlightResult
  export function registerLanguage(name: string, language: unknown): void
  const hljs: {
    highlightElement(element: HTMLElement): void
    highlight(code: string, options: { language: string }): HighlightResult
    highlightAuto(code: string): HighlightResult
    registerLanguage(name: string, language: unknown): void
  }
  export default hljs
}

declare module 'mermaid' {
  export interface MermaidAPI {
    initialize(config: unknown): void
    run(config?: { nodes?: NodeListOf<Element> }): Promise<void>
  }
  export function initialize(config: unknown): void
  export function run(config?: { nodes?: NodeListOf<Element> }): Promise<void>
  const mermaid: MermaidAPI
  export default mermaid
}

declare module 'mathjax/es5/tex-svg-full.js' {
  const MathJax: unknown
  export default MathJax
}

declare module 'anchor-js' {
  export default class AnchorJS {
    options: {
      placement?: string
      visible?: string
      icon?: string
      class?: string
    }

    add(selector?: string): void
    remove(selector?: string): void
  }
}

declare module 'lunr' {
  export interface Index {
    search(query: string): Array<{ ref: string; score: number }>
  }

  export interface Pipeline {
    add(...functions: unknown[]): void
  }

  export interface Builder {
    pipeline: Pipeline
    searchPipeline: Pipeline
    field(name: string, options?: unknown): void
    ref(name: string): void
    add(doc: unknown): void
    use(plugin: (this: Builder) => void): void
    build(): Index
  }
  export const Index: { load(serializedIndex: unknown): Index }
  export const Pipeline: unknown
  export const stemmer: unknown
  export const stopWordFilter: unknown
  export const tokenizer: { separator: RegExp }
  export function multiLanguage(...languages: string[]): (this: Builder) => void
  export default function(callback: (this: Builder, builder: Builder) => void): Index
}

declare module 'lunr-languages/lunr.stemmer.support' {
  export default function(lunr: typeof import('lunr')): void
}

declare module 'lunr-languages/tinyseg' {
  export default function(lunr: typeof import('lunr')): void
}

declare module 'lunr-languages/lunr.multi' {
  export default function(lunr: typeof import('lunr')): void
}

declare module 'lunr-languages/lunr.ar.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.da.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.de.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.du.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.es.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.fi.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.fr.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.hi.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.hu.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.it.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.ja.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.jp.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.ko.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.nl.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.no.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.pt.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.ro.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.ru.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.sa.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.sv.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.ta.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.te.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.th.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.tr.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.vi.js' {
  export default function(lunr: typeof import('lunr')): void
}
declare module 'lunr-languages/lunr.zh.js' {
  export default function(lunr: typeof import('lunr')): void
}

declare module 'idb-keyval' {
  export interface UseStore {
    (): void
  }
  export function get<T = unknown>(key: string, customStore?: UseStore): Promise<T | undefined>
  export function set(key: string, value: unknown, customStore?: UseStore): Promise<void>
  export function del(key: string, customStore?: UseStore): Promise<void>
  export function clear(customStore?: UseStore): Promise<void>
  export function keys(customStore?: UseStore): Promise<string[]>
  export function createStore(dbName: string, storeName: string): UseStore
}

declare module 'bootstrap' {}
declare module 'bootstrap/*' {}

declare module 'node:test' {
  export function describe(name: string, fn: () => void): void
  export function it(name: string, fn: () => void | Promise<void>): void
  export function test(name: string, fn: () => void | Promise<void>): void
  export default function test(name: string, fn: () => void | Promise<void>): void
}

declare module 'node:assert' {
  export function strictEqual<T>(actual: T, expected: T, message?: string): void
  export function deepStrictEqual<T>(actual: T, expected: T, message?: string): void
  export function ok(value: unknown, message?: string): asserts value
  export function fail(message?: string): never
  const assert: {
    strictEqual: typeof strictEqual
    deepStrictEqual: typeof deepStrictEqual
    ok: typeof ok
    fail: typeof fail
  }
  export default assert
}
