<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:m="http://rebar.accenture.com/scorecard/metrics"
                exclude-result-prefixes="msxsl m">

<xsl:key name="metric-lookup" match="m:Metric" use="m:Key"/>

<xsl:variable name="metrics-top" select="document('MetricsThresholds.xml')//m:Metrics"/>

<xsl:template match="/" mode="analyze">
  <xsl:copy>
    <xsl:apply-templates select="@*|node()" mode="analyze">
      <xsl:with-param name="counter" select="number(1)" />
    </xsl:apply-templates>
  </xsl:copy>
</xsl:template>

<xsl:template match="@*|node()" mode="analyze">
  <xsl:param name="counter" />

  <xsl:copy>
    <xsl:apply-templates select="@*|node()" mode="analyze">
      <xsl:with-param name="counter" select="$counter" />
    </xsl:apply-templates>
  </xsl:copy>
</xsl:template>

  <xsl:template match="CodeMetricsReport" mode="analyze">
    <xsl:copy>
      <xsl:attribute name="TotalSteps">
        <xsl:value-of select="count(//Metric)"/>
      </xsl:attribute>
    <xsl:apply-templates select="@*|node()" mode="analyze">
      <xsl:with-param name="counter" select="number(1)" />
    </xsl:apply-templates>
  </xsl:copy>
    
  </xsl:template>
  <xsl:template match="Metric" mode="analyze">
    <xsl:param name="counter" />

    <xsl:copy>
      <xsl:variable name="metric-threshold-frag">
        <xsl:apply-templates select="$metrics-top" mode="lookup-metric-threshold">
          <xsl:with-param name="current-node" select="." />
        </xsl:apply-templates>
      </xsl:variable>    
      
      <xsl:attribute name="Parent-Position">
        <xsl:value-of select="count(parent::*/parent::*/parent::*/parent::*/Metrics/Metric)" />
      </xsl:attribute>
      
      <xsl:attribute name="Parent-Parent-Position">
        <xsl:value-of select="count(parent::*/parent::*/preceding-sibling::*/parent::*/parent::*/preceding-sibling::*//Metrics) + 1" />
      </xsl:attribute>
      
      <xsl:attribute name="Parent-Parent-Parent-Position">
        <xsl:value-of select="count(parent::*/parent::*/preceding-sibling::*/parent::*/parent::*/preceding-sibling::*/parent::*/parent::*/preceding-sibling::*//Metrics) + 1" />
      </xsl:attribute>
      
      <!-- add a label -->
      <xsl:attribute name="Label">
        <xsl:apply-templates select="$metrics-top" mode="lookup-metric-name">
          <xsl:with-param name="current-node" select="." />
        </xsl:apply-templates>
      </xsl:attribute>

      <xsl:variable name="metric-status">
        <xsl:apply-templates select="$metrics-top" mode="lookup-metric-status">
          <xsl:with-param name="current-node" select="." />
        </xsl:apply-templates>
      </xsl:variable>

      <xsl:variable name="metric-status-priority">
        <xsl:choose>
          <xsl:when test="$metric-status = 'error'">
            <xsl:value-of select="number(10)"/>
          </xsl:when>
          <xsl:when test="$metric-status = 'warn'">
            <xsl:value-of select="number(20)"/>
          </xsl:when>
          <xsl:when test="$metric-status = 'pass'">
            <xsl:value-of select="number(30)"/>
          </xsl:when>
          <xsl:when test="$metric-status = 'NA'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:when test="$metric-status = 'UNKNOWN'">
            <xsl:value-of select="number(100)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="number(1000)"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>

      <!-- add the scorecard status, pass, warning, error, NA or UNKNOWN -->
      <xsl:attribute name="Status">
        <xsl:value-of select="$metric-status"/>
      </xsl:attribute>

      <xsl:attribute name="Status-Priority">
        <xsl:value-of select="$metric-status-priority"/>
      </xsl:attribute>

      <!-- add a scope attribute to the metric so finding it will be easier 
                 The scope value is the element name of the parent node that metric is
                 contained in -->
      <xsl:attribute name="Scope">
        <!-- <Type><Metrics><Metric /></Metrics></Type>  -->
        <xsl:value-of select="local-name(../..)"/>
      </xsl:attribute>

      <xsl:variable name="metric-threshold" select="msxsl:node-set($metric-threshold-frag)" />
      <xsl:if test="$metric-threshold/*">
        <xsl:attribute name="ErrorThreshold">
          <xsl:value-of select="$metric-threshold//m:Level[m:Key = 'Error']/m:Threshold" />
        </xsl:attribute>

        <xsl:attribute name="WarningThreshold">
          <xsl:value-of select="$metric-threshold//m:Level[m:Key = 'Warn']/m:Threshold"/>
        </xsl:attribute>

        <xsl:attribute name="RecommendedThreshold">
          <xsl:value-of select="$metric-threshold//m:TargetValue"/>
        </xsl:attribute>
      </xsl:if>
      
      <xsl:apply-templates select="@*|node()" mode="analyze">
        <xsl:with-param name="counter" select="$counter + 1" />
      </xsl:apply-templates>
    </xsl:copy>
  </xsl:template>

  <xsl:template name="lookup-metric-status" match="m:Metrics" mode="lookup-metric-status">
    <xsl:param name="current-node" />
    <xsl:variable name="metric" select="key('metric-lookup', $current-node/@Name)" />
    <xsl:variable name="metric-scope" select="local-name($current-node/../..)" />
    <xsl:variable name="metric-threshold" select="$metric/m:Thresholds/m:Threshold[m:Scope = $metric-scope]" />
    <xsl:variable name="metric-value" select="number($current-node/@Value)" />
    <xsl:variable name="error-value" select="number($metric-threshold//m:Level[m:Key = 'Error']/m:Threshold)" />
    <xsl:variable name="warning-value" select="number($metric-threshold//m:Level[m:Key = 'Warn']/m:Threshold)" />
    <xsl:variable name="metric-rating" select="$metric-threshold/m:Comparison" />

    <xsl:choose>
      <xsl:when test="not($metric-threshold)">
        <xsl:value-of select="'NA'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Max' and $metric-value &gt; $error-value">
        <xsl:value-of select="'error'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Max' and $metric-value &lt;= $error-value and $metric-value &gt;= $warning-value">
        <xsl:value-of select="'warn'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Max' and $metric-value &lt; $warning-value">
        <xsl:value-of select="'pass'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Min' and $metric-value &lt; $error-value">
        <xsl:value-of select="'error'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Min' and $metric-value &gt;= $error-value and $metric-value &lt;= $warning-value">
        <xsl:value-of select="'warn'"/>
      </xsl:when>
      <xsl:when test="$metric-rating = 'Min' and $metric-value &gt; $warning-value">
        <xsl:value-of select="'pass'"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="'UNKNOWN'"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="lookup-metric-name" match="m:Metrics" mode="lookup-metric-name">
    <xsl:param name="current-node" />
    <xsl:value-of select="key('metric-lookup', $current-node/@Name)/m:Label"/>
  </xsl:template>

  <xsl:template name="lookup-metric-threshold" match="m:Metrics" mode="lookup-metric-threshold">
    <xsl:param name="current-node" />
    <xsl:variable name="metric" select="key('metric-lookup', $current-node/@Name)" />
    <xsl:variable name="metric-threshold" select="$metric/m:Thresholds/m:Threshold[m:Scope = local-name($current-node/../..)]" />
    <xsl:copy-of select="$metric-threshold" />
  </xsl:template>

  <xsl:template name="create-fqn-id">
    <xsl:param name="name" />
    <xsl:param name="context" />

    <xsl:variable name="value">
      <xsl:call-template name="create-fqn">
        <xsl:with-param name="name" select="$name" />
        <xsl:with-param name="context" select="$context" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="translate($value, '&lt;&gt;#@() ', '::_____')"/>
  </xsl:template>

  <xsl:template name="create-fqn">
    <xsl:param name="name" />
    <xsl:param name="context" />

    <xsl:variable name="ancestor" select="$context/../.." />

    <xsl:choose>
      <xsl:when test="$ancestor/@Name and not(local-name($ancestor) = 'Target')">
        <xsl:call-template name="create-fqn">
          <xsl:with-param name="name" select="concat($ancestor/@Name, '.', $name)"/>
          <xsl:with-param name="context" select="$ancestor/../.." />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$name" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <!-- Prints text with <br/> at line breaks.
       Also introduces <wbr/> word break characters every so often if no natural breakpoints appear.
       Some of the soft breaks are useful, but the forced break at 20 chars is intended to work around
       limitations in browsers that do not support the word-wrap:break-all CSS3 property.
  -->
  <xsl:template name="print-text-with-breaks">
    <xsl:param name="text" />
    <xsl:param name="count" select="0" />
    
    <xsl:if test="$text">
      <xsl:variable name="char" select="substring($text, 1, 1)"/>
      
      <xsl:choose>
        <!-- natural word breaks -->
        <xsl:when test="$char = ' '">
          <!-- Always replace spaces by non-breaking spaces followed by word-breaks to ensure that
               text can reflow without actually consuming the space.  Without this detail
               it can happen that spaces that are supposed to be highligted (perhaps as part
               of a marker for a diff) will instead vanish when the text reflow occurs, giving
               a false impression of the content. -->
          <xsl:text>&#160;</xsl:text>
          <wbr/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="0" />
          </xsl:call-template>
        </xsl:when>

        <!-- line breaks -->
        <xsl:when test="$char = '&#10;'">
          <br/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="0" />
          </xsl:call-template>
        </xsl:when>
        
        <!-- characters to break before -->
        <xsl:when test="$char = '.' or $char = '/' or $char = '\' or $char = ':' or $char = '(' or $char = '&lt;' or $char = '[' or $char = '{' or $char = '_'">
          <wbr/>
          <xsl:value-of select="$char"/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="1" />
          </xsl:call-template>
        </xsl:when>
        
        <!-- characters to break after -->
        <xsl:when test="$char = ')' or $char = '>' or $char = ']' or $char = '}'">
          <xsl:value-of select="$char"/>
          <wbr/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="0" />
          </xsl:call-template>
        </xsl:when>
        
        <!-- other characters -->
        <xsl:when test="$count = 19">
          <xsl:value-of select="$char"/>
          <wbr/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="0" />
          </xsl:call-template>
        </xsl:when>
        
        <xsl:otherwise>
          <xsl:value-of select="$char"/>
          <xsl:call-template name="print-text-with-breaks">
            <xsl:with-param name="text" select="substring($text, 2)" />
            <xsl:with-param name="count" select="$count + 1" />
          </xsl:call-template>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  
</xsl:stylesheet>
