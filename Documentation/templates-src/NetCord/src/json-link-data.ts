// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/**
 * JSON-LD (JSON for Linking Data) structured data injection for SEO and rich snippets.
 * Automatically detects page type and injects appropriate schema.org markup.
 */

interface Breadcrumb {
  name: string
  url: string
}

interface Metadata {
  title: string
  description: string
  url: string
  breadcrumbs: Breadcrumb[]
}

interface JsonLinkDataSchema {
  '@context': string
  '@type': string
  [key: string]: unknown
}

export async function initializeJsonLinkData () {
  injectStructuredData()
}

function injectStructuredData (): void {
  const metadata = gatherMetadata()

  // Determine page type based on URL path
  const path = window.location.pathname
  let schema: JsonLinkDataSchema

  if (path.indexOf('/guides/') !== -1) {
    schema = createTechArticleSchema(metadata)
  } else if (path.indexOf('/api/') !== -1) {
    schema = createSoftwareApplicationSchema(metadata)
  } else {
    schema = createWebPageSchema(metadata)
  }

  // Inject JSON-LD script tag into document head
  const script = document.createElement('script')
  script.type = 'application/ld+json'
  script.textContent = JSON.stringify(schema, null, 2)
  document.head.appendChild(script)
}

function gatherMetadata (): Metadata {
  const title = document.title
  const descriptionElement = document.querySelector('meta[name="description"]')
  const description = descriptionElement?.getAttribute('content') || 'Modern and fully customizable C# Discord library'
  const url = window.location.href
  const breadcrumbs = extractBreadcrumbs()

  return { title, description, url, breadcrumbs }
}

function extractBreadcrumbs (): Breadcrumb[] {
  const breadcrumbs: Breadcrumb[] = []

  // Try to extract from breadcrumb navigation if present
  const breadcrumbNavigation = document.querySelector('nav[aria-label="breadcrumb"]')
  if (breadcrumbNavigation) {
    const items = breadcrumbNavigation.querySelectorAll('ol > li')
    items.forEach((item) => {
      const link = item.querySelector('a')
      if (link) {
        breadcrumbs.push({
          name: link.textContent || '',
          url: link.getAttribute('href') || ''
        })
      } else {
        // Last item might not be a link
        const text = item.textContent || ''
        if (text.trim()) {
          breadcrumbs.push({
            name: text.trim(),
            url: window.location.href
          })
        }
      }
    })
  }

  return breadcrumbs
}

function createTechArticleSchema (metadata: Metadata): JsonLinkDataSchema {
  const publishDate = document.querySelector('meta[property="article:published_time"]')?.getAttribute('content') || new Date().toISOString()
  const modifiedDate = document.querySelector('meta[property="article:modified_time"]')?.getAttribute('content') || publishDate
  const image = document.querySelector('meta[property="og:image"]')?.getAttribute('content')

  return {
    '@context': 'https://schema.org',
    '@type': 'TechArticle',
    headline: metadata.title,
    description: metadata.description,
    url: metadata.url,
    datePublished: publishDate,
    dateModified: modifiedDate,
    ...(image && { image }),
    keywords: extractKeywords(),
    author: {
      '@type': 'Organization',
      name: 'NetCord',
      url: 'https://netcord.dev',
      logo: {
        '@type': 'ImageObject',
        url: 'https://netcord.dev/logo.png',
        width: 250,
        height: 60
      }
    },
    publisher: {
      '@type': 'Organization',
      name: 'NetCord',
      url: 'https://netcord.dev',
      logo: {
        '@type': 'ImageObject',
        url: 'https://netcord.dev/logo.png',
        width: 250,
        height: 60
      }
    },
    articleSection: 'Technology',
    ...(metadata.breadcrumbs.length > 0 && {
      breadcrumb: createBreadcrumbList(metadata.breadcrumbs)
    })
  }
}

function createSoftwareApplicationSchema (_metadata: Metadata): JsonLinkDataSchema {
  return {
    '@context': 'https://schema.org',
    '@type': 'SoftwareApplication',
    name: 'NetCord',
    description: 'Modern and fully customizable C# Discord library for building Discord bots with .NET',
    url: 'https://netcord.dev',
    applicationCategory: 'DeveloperApplication',
    softwareVersion: '0.1.0',
    releaseNotes: 'https://github.com/NetCordDev/NetCord/releases',
    downloadUrl: 'https://www.nuget.org/packages/NetCord',
    operatingSystem: 'Windows, macOS, Linux',
    programmingLanguage: 'C#',
    author: {
      '@type': 'Organization',
      name: 'NetCord Community',
      url: 'https://github.com/NetCordDev/NetCord'
    },
    publisher: {
      '@type': 'Organization',
      name: 'NetCord',
      url: 'https://netcord.dev'
    },
    screenshot: 'https://netcord.dev/screenshot.png'
  }
}

function createWebPageSchema (metadata: Metadata): JsonLinkDataSchema {
  const image = document.querySelector('meta[property="og:image"]')?.getAttribute('content')

  return {
    '@context': 'https://schema.org',
    '@type': 'WebPage',
    name: metadata.title,
    description: metadata.description,
    url: metadata.url,
    keywords: extractKeywords(),
    ...(image && { image }),
    publisher: {
      '@type': 'Organization',
      name: 'NetCord',
      url: 'https://netcord.dev',
      logo: {
        '@type': 'ImageObject',
        url: 'https://netcord.dev/logo.png',
        width: 250,
        height: 60
      }
    },
    ...(metadata.breadcrumbs.length > 0 && {
      breadcrumb: createBreadcrumbList(metadata.breadcrumbs)
    })
  }
}

function createBreadcrumbList (breadcrumbs: Breadcrumb[]): JsonLinkDataSchema {
  return {
    '@context': 'https://schema.org',
    '@type': 'BreadcrumbList',
    itemListElement: breadcrumbs.map((crumb, index) => ({
      '@type': 'ListItem',
      position: index + 1,
      name: crumb.name,
      item: crumb.url
    }))
  }
}

function extractKeywords (): string {
  const keywordsMeta = document.querySelector('meta[name="keywords"]')?.getAttribute('content')
  if (keywordsMeta) return keywordsMeta

  // Fallback: extract from page content based on path
  const path = window.location.pathname
  if (path.includes('/guides/')) {
    return 'Discord, bot, C#, .NET, tutorial, guide'
  } else if (path.includes('/api/')) {
    return 'Discord API, C#, documentation, reference'
  }
  return 'Discord, C#, .NET, library, documentation'
}
