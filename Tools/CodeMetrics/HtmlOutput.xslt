<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:m="http://rebar.accenture.com/scorecard/metrics"
    exclude-result-prefixes="msxsl m">

    <xsl:output method="html" indent="yes"  encoding="utf-8"/>
    <xsl:strip-space elements="Metrics" />
    <xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
    <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
  
    <xsl:include href="CodeMetricsAnalyzer.xslt" />
  
  <xsl:template match="/">
    <!-- Run a pre-analysis step to mutate the code metrics report
        adding extra metadata and calculating the pass/warn/error failures
        for each metric, store this in an xml fragment so we can use it
        for transforming the actual result -->
    <xsl:variable name="code-metrics-frag">
      <xsl:apply-templates select="/" mode="analyze" />
    </xsl:variable>

    <!--<xsl:copy>
      <xsl:apply-templates select="/" mode="analyze" />
    </xsl:copy>-->

    <xsl:variable name="code-metrics" select="msxsl:node-set($code-metrics-frag)" />
    <xsl:apply-templates select="$code-metrics" mode="html" />
  </xsl:template>

  <xsl:template match="/" mode="html">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>
    <html lang='en'>
      <xsl:call-template name="html-head" />
      <body class="scorecard-report">

        <div id="header">
          <h1>Accenture REBAR Code Metrics</h1>
        </div>
        
        <div id="navigator">
          <a href="#scorecard" class="navigator-box status-error" title="Top" ></a>
          <div class="navigator-stripes">
          <xsl:for-each select="//Module|//Namespace|//Type|//Member">
            <xsl:choose>
              <xsl:when test="Metrics/Metric[@Status = 'error']">
                <a title="Errors found for {@Name}" href="#{generate-id()}-detail" style="top:{position() * 98 div last() + 1}%" class="status-error"></a>
              </xsl:when>
              <xsl:when test="Metrics/Metric[@Status = 'warn']">
                <a title="Warnings found for {@Name}" href="#{generate-id()}-detail" style="top:{position() * 98 div last() + 1}%" class="status-error"></a>
              </xsl:when>
            </xsl:choose>
          </xsl:for-each>
          </div>
        </div>

        <div class="content">
          <div id="scorecard">
            <h1>Score Card</h1>
            <ul>
              <li><xsl:call-template name="html-scorecard-table" /></li>
            </ul>
          </div>
          
          <xsl:call-template name="html-summary" />
          <xsl:call-template name="html-details" />

        </div>
        <xsl:copy-of select="document('jquery.html')" />
        <script type="text/javascript">
          <![CDATA[
          jQuery(document).ready(function ($) {

            $('a.toggle.plus').each(collapse);

            function collapse() {
              $(this).removeClass("minus").addClass("plus").attr("title", $(this).hasClass("minus") ? "collapse" : "expand");
              $(this).closest("li").children(".panel").first().slideUp();
            }

            function expand() {
              $(this).removeClass("plus").addClass("minus").attr("title", $(this).hasClass("minus") ? "collapse" : "expand");
              $(this).closest("li").children(".panel").first().slideDown();
            }

            $("#master-toggle").click(function () {
              var shouldCollapse = $(this).hasClass("minus");
              $("section a.toggle:first-child").each(shouldCollapse ? collapse : expand);
              $("a.toggle").each(shouldCollapse ? collapse : expand);
              $(this).toggleClass("minus plus");
            });

            $("a.toggle").click(function () {
              var $e = $(this);
              $e.toggleClass("minus plus").attr("title", $e.hasClass("minus") ? "collapse" : "expand");
              $e.closest("li").children(".panel").first().slideToggle();
              return false;
            });
          });
          ]]>
        </script>
      </body>
    </html>
  </xsl:template>
  
  <xsl:template name="gather-scope-status-indicator" match="Module|Namespace|Type|Member" mode="status-indicator">
    <xsl:variable name="modules-status">
     <xsl:for-each select="descendant-or-self::Module/Metrics/Metric">
       <xsl:sort data-type="number" order="ascending" select="@Status-Priority"/>
       <xsl:if test="position()=1"><xsl:value-of select="@Status"/></xsl:if>
     </xsl:for-each>
   </xsl:variable>

    <xsl:variable name="namespaces-status">
     <xsl:for-each select="descendant-or-self::Namespace/Metrics/Metric">
       <xsl:sort data-type="number" order="ascending" select="@Status-Priority"/>
       <xsl:if test="position()=1"><xsl:value-of select="@Status"/></xsl:if>
     </xsl:for-each>
   </xsl:variable>

    <xsl:variable name="types-status">
     <xsl:for-each select="descendant-or-self::Type/Metrics/Metric">
       <xsl:sort data-type="number" order="ascending" select="@Status-Priority"/>
       <xsl:if test="position()=1"><xsl:value-of select="@Status"/></xsl:if>
     </xsl:for-each>
   </xsl:variable>

    <xsl:variable name="members-status">
     <xsl:for-each select="descendant-or-self::Member/Metrics/Metric">
       <xsl:sort data-type="number" order="ascending" select="@Status-Priority"/>
       <xsl:if test="position()=1"><xsl:value-of select="@Status"/></xsl:if>
     </xsl:for-each>
   </xsl:variable>


    <xsl:if test="descendant-or-self::Module">
      <xsl:call-template name="output-scope-status-indicator">
        <xsl:with-param name="status" select="$modules-status" />
        <xsl:with-param name="scope" select="'Module'" />
      </xsl:call-template>
    </xsl:if>
    
    <xsl:if test="descendant-or-self::Namespace">
      <xsl:call-template name="output-scope-status-indicator">
        <xsl:with-param name="status" select="$namespaces-status" />
        <xsl:with-param name="scope" select="'Namespace'" />
      </xsl:call-template>
    </xsl:if>
    
    <xsl:if test="descendant-or-self::Type">
    <xsl:call-template name="output-scope-status-indicator">
      <xsl:with-param name="status" select="$types-status" />
      <xsl:with-param name="scope" select="'Type'" />
    </xsl:call-template>
    </xsl:if>
    
    <xsl:if test="descendant-or-self::Member">
    <xsl:call-template name="output-scope-status-indicator">
      <xsl:with-param name="status" select="$members-status" />
      <xsl:with-param name="scope" select="'Member'" />
    </xsl:call-template>
    </xsl:if>

  </xsl:template>
  
  <xsl:template name="output-scope-status-indicator">
    <xsl:param name="status" />
    <xsl:param name="scope" />

    <xsl:if test="$status">
      <span>
        <xsl:attribute name="class">
          <xsl:value-of select="concat('status-indicator status-',$status)"/>
        </xsl:attribute>
        <xsl:attribute name="title">
          <xsl:choose>
            <xsl:when test="$status = 'error'">
              <xsl:value-of select="concat('Errors found at the ', $scope, ' level')"/>
            </xsl:when>
            <xsl:when test="$status = 'warn'">
              <xsl:value-of select="concat('Warnings found at the ', $scope, ' level')"/>
            </xsl:when>
            <xsl:when test="$status = 'pass'">
              <xsl:value-of select="concat('No issues found at the ', $scope, ' level')"/>
            </xsl:when>
            <xsl:when test="$status = 'NA'">
              <xsl:value-of select="concat('No metrics are being tracked at the ', $scope, ' level')"/>
            </xsl:when>
          </xsl:choose>

        </xsl:attribute>
        <xsl:choose>
          <xsl:when test="$status = 'NA'">
            <xsl:text disable-output-escaping="yes">&amp;#x25A1;</xsl:text>
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text disable-output-escaping="yes">&amp;#x25A0;</xsl:text>
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </span>
    </xsl:if>
  </xsl:template>

  <xsl:template name="html-head">
    
      <head>
        <meta charset="utf-8" />
		    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />

        <title>REBAR Scorecard Report</title>

		    <!-- Meta tags -->
		    <meta name="description" content="" />
		    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <xsl:copy-of select="document('ScoreCardStyleFragment.html')" />
    </head>
  </xsl:template>

  <xsl:template name="html-scoped-entry" match="Module|Namespace|Type|Member" mode="output">
    <li>
      <xsl:call-template name="entry-title" />
      <div class="panel">
        <xsl:if test="local-name() = 'Member'">
          <xsl:attribute name="style">
            <xsl:value-of select="'display: none;'"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:call-template name="html-scoped-entry-metadata" />
        <xsl:apply-templates select="." mode="output-detail"/>
      </div>
    </li>
  </xsl:template>
  
  <xsl:template match="Module|Namespace" mode="output-detail">
    <xsl:call-template name="html-scorecard-table" />
    <xsl:call-template name="html-metrics-table" />
    <ul>
      <xsl:apply-templates select="*[not(self::Metrics)]/*" mode="output" />
    </ul>
  </xsl:template>

  <xsl:template match="Type|Member" mode="output-detail">
    <xsl:call-template name="html-metrics-table" />
    <ul>
      <xsl:apply-templates select="*[not(self::Metrics)]/*" mode="output" />
    </ul>
  </xsl:template>
  
  <xsl:template name="html-scoped-entry-metadata">
    <xsl:choose>
      <xsl:when test="local-name() = 'Module'">
        <ul class="metadata">
          <li><xsl:value-of select="concat('Code Base: ', ../../@Name)"/></li>
          <li><xsl:value-of select="concat('Version: ', @AssemblyVersion)"/></li>
          <li><xsl:value-of select="concat('File Version: ', @FileVersion)"/></li>
        </ul>
      </xsl:when>
      <xsl:when test="local-name() = 'Member' and @File and @Line">
        <ul class="metadata">
          <li><xsl:value-of select="concat('File: ', @File)"/></li>
          <li><xsl:value-of select="concat('Line: ', @Line)"/></li>
        </ul>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="html-metrics-table">
    <table class="metrics">
      <caption><xsl:value-of select="concat(local-name(), ' Metrics')"/></caption>
      <thead>
        <tr>
          <xsl:for-each select="./Metrics/Metric">
            <th>
              <xsl:apply-templates select="$metrics-top" mode="lookup-metric-name">
                <xsl:with-param name="current-node" select="." />
              </xsl:apply-templates>
            </th>
          </xsl:for-each>
        </tr>
      </thead>
      <tbody>
        <tr>
          <xsl:for-each select="./Metrics/Metric">
            <td>
              <xsl:value-of select="@Value"/>
              <span class="status-{@Status}">
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <xsl:choose>
                <xsl:when test="@Status = 'error' or @Status = 'warn' or @Status = 'pass'">
                  <xsl:text disable-output-escaping="yes">&amp;#x25A0;</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text disable-output-escaping="yes">&amp;#x25A1;</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
              </span>
            </td>
          </xsl:for-each>
        </tr>
      </tbody>
    </table>
  </xsl:template>
 
  <xsl:template name="entry-title">
    <xsl:param name="suffix" select="'detail'" />
    
    <xsl:variable name="level">
      <xsl:choose>
        <xsl:when test="local-name() = 'Module'"><xsl:value-of select="'2'"/></xsl:when>
        <xsl:when test="local-name() = 'Namespace'"><xsl:value-of select="'3'"/></xsl:when>
        <xsl:when test="local-name() = 'Type'"><xsl:value-of select="'4'"/></xsl:when>
        <xsl:when test="local-name() = 'Member'"><xsl:value-of select="'5'"/></xsl:when>
      </xsl:choose>
    </xsl:variable>
    
    <xsl:element name="h{$level}">
      <a href="#" title="collapse" class="toggle icon minus">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="number($level) = 5"><xsl:value-of select="'toggle icon plus'" /></xsl:when>
            <xsl:otherwise><xsl:value-of select="'toggle icon minus'"/></xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:attribute name="title">
          <xsl:choose>
            <xsl:when test="number($level) = 5"><xsl:value-of select="'expand'" /></xsl:when>
            <xsl:otherwise><xsl:value-of select="'collapse'"/></xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
      </a>
      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      <span>
        <xsl:attribute name="class">
          <xsl:value-of select="concat('icon scope-', translate(local-name(), $uppercase, $lowercase))"/>
        </xsl:attribute>
      </span>
      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      <xsl:apply-templates select="." mode="status-indicator" />
      <a id="{generate-id()}-{$suffix}" name="{generate-id()}-{$suffix}"><xsl:value-of select="@Name"/></a>
    </xsl:element>
  </xsl:template>
  
  <xsl:template name="html-details">
    <div id="details">
      <h1>Details</h1>
      <ul>
        <xsl:for-each select="//Module">
          <xsl:call-template name="html-scoped-entry" />
        </xsl:for-each>
        
      </ul>
    </div>
  </xsl:template>
 
  <xsl:template name="html-summary">
    <div id="summary">
      <h1>Summary</h1>
      <ul>
        <xsl:for-each select="//Module">
          <li>
            <h2>
              <a href="#" title="collapse" class="toggle icon minus"></a>
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <span class="icon scope-module"></span>
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <xsl:apply-templates select="." mode="status-indicator" />
              <a href="#{generate-id()}-detail"><xsl:value-of select="@Name"/></a>
            </h2>
            <div class="panel">
              <ul>
                <xsl:for-each select="./Namespaces/Namespace">
                  <li>
                    <h3>
                      <a href="#" title="collapse" class="toggle icon minus"></a>
                      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                      <span class="icon scope-namespace"></span>
                      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                      <xsl:apply-templates select="." mode="status-indicator" />
                      <a href="#{generate-id()}-detail"><xsl:value-of select="@Name"/></a>
                    </h3>
                          
                    <div class="panel">
                      <ul>
                        <xsl:for-each select="./Types/Type[./Metrics/Metric[@Status = 'error'] or ./Metrics/Metric[@Status = 'warn'] or ./Members/Member/Metrics/Metric[@Status = 'error'] or ./Members/Member/Metrics/Metric[@Status = 'warn']]">
                          <li>
                            <h4>
                              <a href="#" title="expand" class="toggle icon plus"></a>
                              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                              <span class="icon scope-type"></span>
                              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                              <xsl:apply-templates select="." mode="status-indicator" />
                              <a href="#{generate-id()}-detail"><xsl:value-of select="@Name"/></a>
                            </h4>
                            
                            <div class="panel">
                              <ul>
                                <xsl:for-each select="./Members/Member[./Metrics/Metric[@Status = 'error'] or ./Metrics/Metric[@Status = 'warn']]">
                                  <li>
                                    <h5>
                                      <a href="#" class="icon full-stop"></a>
                                      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                                      <span class="icon scope-member"></span>
                                      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                                      <xsl:apply-templates select="." mode="status-indicator" />
                                      <a href="#{generate-id()}-detail"><xsl:value-of select="@Name"/></a>
                                    </h5>
                                  </li>
                                </xsl:for-each>
                              </ul>
                            </div>
                            
                          </li>
                        </xsl:for-each>
                      </ul>
                    </div>
                  </li>
                </xsl:for-each>
              </ul>
            </div>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
  
  <xsl:template name="html-scorecard-table">
    <xsl:variable name="metrics" select="." />

    <xsl:variable name="scoped-name">
      <xsl:choose>
        <xsl:when test="local-name() = 'Module' or local-name() = 'Namespace' or local-name() = 'Type' or local-name() = 'Member'">
          <xsl:value-of select="@Name"/>
        </xsl:when>
        <xsl:otherwise><xsl:value-of select="REBAR"/></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="total-error-count" select="count($metrics//Metric[@Status = 'error'])" />
    <xsl:variable name="total-warning-count" select="count($metrics//Metric[@Status = 'warn'])" />
    <xsl:variable name="total-pass-count" select="count($metrics//Metric[@Status = 'pass'])" />
    <xsl:variable name="total-summary-count" select="$total-error-count + $total-warning-count + $total-pass-count" />

      <table>
        <caption><xsl:value-of select="concat($scoped-name, ' Score Card')"/></caption>
        <thead>
          <tr>
            <th>Metric</th>
            <th>Scope</th>
            <th>Total</th>
            <th>Errors</th>
            <th>Error %</th>
            <th>Warnings</th>
            <th>Warning %</th>
            <th>Pass</th>
            <th>Pass %</th>
          </tr>
        </thead>
        <tfoot>
          <tr>
            <th colspan="2">Total</th>
            <td><xsl:value-of select="$total-summary-count"/></td>
            <td><xsl:value-of select="$total-error-count"/></td>
            <td><xsl:value-of select="format-number($total-error-count div $total-summary-count, '0.###%')"/></td>
            <td><xsl:value-of select="$total-warning-count"/></td>
            <td><xsl:value-of select="format-number($total-warning-count div $total-summary-count, '0.###%')"/></td>
            <td><xsl:value-of select="$total-pass-count"/></td>
            <td><xsl:value-of select="format-number($total-pass-count div $total-summary-count, '0.###%')"/></td>
          </tr>
        </tfoot>
        <tbody>
          <!-- Loop over each metric that should be displayed in the totals, and calc the totals -->
          <xsl:for-each select="$metrics-top//m:Threshold[m:DisplayInScorecard = 'True']">
            <xsl:variable name="metric-key" select="../../m:Key" />
            <xsl:variable name="metric-scope" select="m:Scope" />

            <xsl:variable name="error-count" select="count($metrics//Metric[@Name = $metric-key and @Scope = $metric-scope and @Status = 'error'])" />
            <xsl:variable name="warning-count" select="count($metrics//Metric[@Name = $metric-key and @Scope = $metric-scope and @Status = 'warn'])" />
            <xsl:variable name="pass-count" select="count($metrics//Metric[@Name = $metric-key and @Scope = $metric-scope and @Status = 'pass'])" />
            <xsl:variable name="total-count" select="$error-count + $warning-count + $pass-count" />

            <tr>
              <th><xsl:value-of select="../../m:Label"/></th>
              <th><xsl:value-of select="$metric-scope"/></th>
              <td><xsl:value-of select="$total-count"/></td>
              <td><xsl:value-of select="$error-count"/></td>
              <td><xsl:value-of select="format-number(number($error-count div $total-count), '0.###%')"/></td>
              <td><xsl:value-of select="$warning-count"/></td>
              <td><xsl:value-of select="format-number(number($warning-count div $total-count), '0.###%')"/></td>
              <td><xsl:value-of select="$pass-count"/></td>
              <td><xsl:value-of select="format-number(number($pass-count div $total-count), '0.###%')"/></td>
            </tr>
          </xsl:for-each>
        </tbody>
        </table>
  </xsl:template>
  
</xsl:stylesheet>
