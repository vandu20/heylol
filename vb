package com.db.fusion.rs.extractor;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import javax.ejb.EJB;
import javax.ejb.Stateful;
import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;
import org.apache.logging.log4j.Logger;
import org.apache.logging.log4j.LogManager;

@Stateful
public class RSReportGenerator {

    private static final Logger LOGGER = LogManager.getLogger(RSReportGenerator.class);

    private final String SEPARATOR = ",";
    
    @EJB
    private RSDao rsDao;

    private String restrictionType;
    private String overrideRestrictionTypeName;

    // SQL Queries
    private static final String SQL_RESTRICTION_TYPE = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_TYPE_NAME +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " v WHERE v." + ExtractorConstants.RESTRICTION_TYPE_CODE + " = ?" +
            " AND ROWNUM <= 1 AND " + ExtractorConstants.STATUS_CODE + " = ?";

    private static final String SQL_RESTRICTIONS = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_ID +
            ", v." + ExtractorConstants.SECURITY_NAME +
            ", v." + ExtractorConstants.INSTRUMENT_ID_TYPE +
            ", (CASE WHEN v." + ExtractorConstants.INSTRUMENT_ID_TYPE + " = ?" +
            " THEN SUBSTR(v." + ExtractorConstants.INSTRUMENT_ID_VALUE + ", 1, " + ExtractorConstants.RIC_MAX_SIZE + ")" +
            " ELSE v." + ExtractorConstants.INSTRUMENT_ID_VALUE + " END) AS " + ExtractorConstants.INSTRUMENT_ID_VALUE +
            " FROM (SELECT * FROM (SELECT DISTINCT t." + ExtractorConstants.INSTRUMENT_ISIN + " AS " + ExtractorConstants.RESTRICTION_ID +
            ", t." + ExtractorConstants.INSTRUMENT_ISIN + ", " + ExtractorConstants.INSTRUMENT_CUSIP +
            ", t." + ExtractorConstants.INSTRUMENT_WKN + ", " + ExtractorConstants.INSTRUMENT_RIC +
            ", t." + ExtractorConstants.SECURITY_NAME +
            " FROM " + ExtractorConstants.VW_RESTRICTION + " t" +
            " WHERE t." + ExtractorConstants.STATUS_CODE + " = ?" +
            " AND t." + ExtractorConstants.INSTRUMENT_ISIN + " IS NOT NULL" +
            " AND t." + ExtractorConstants.RESTRICTION_TYPE_CODE + " = ?" +
            ") UNPIVOT(" + ExtractorConstants.INSTRUMENT_ID_VALUE + " FOR " + ExtractorConstants.INSTRUMENT_ID_TYPE +
            " IN (" + ExtractorConstants.INSTRUMENT_ISIN + " AS '" + ExtractorConstants.ISIN + "', " +
            ExtractorConstants.INSTRUMENT_CUSIP + " AS '" + ExtractorConstants.CUSIP + "', " +
            ExtractorConstants.INSTRUMENT_WKN + " AS '" + ExtractorConstants.WPK + "', " +
            ExtractorConstants.INSTRUMENT_RIC + " AS '" + ExtractorConstants.RIC + "'))) v" +
            " ORDER BY " + ExtractorConstants.RESTRICTION_ID;

    // Method to set restriction type
    public void setRestrictionType(String restrictionType, String overrideRestrictionTypeName) {
        this.restrictionType = restrictionType;
        this.overrideRestrictionTypeName = overrideRestrictionTypeName;
    }

    // Method to generate XML report
    public void generateReport(XMLStreamWriter xtw) throws Exception {
        Validate.notNull(restrictionType, "Restriction type is not specified");

        StaXBuilder staXBuilder = new StaXBuilder(xtw);

        String defaultRestrictionType = ExtractorConstants.DEFAULT_RESTRICTION_TYPE;
        if (overrideRestrictionTypeName != null) {
            defaultRestrictionType = overrideRestrictionTypeName;
        } else {
            defaultRestrictionType = rsDao.getRestrictionType(restrictionType);  // Get the restriction type from the database
        }

        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> restrictions = rsDao.getRestrictions(restrictionType, incorrectRICs); // Get list of restrictions from DB

        // Create report details
        staXBuilder.startElement(ExtractorConstants.REPORT_DETAILS)
                .addElement(ExtractorConstants.TYPE_OF_RESTRICTION, defaultRestrictionType)
                .addElement(ExtractorConstants.EXTRACT_TIMESTAMP, DateTimeHelper.formatCalendarGmt(DateTimeHelper.getGMTCalendar()))
                .addElement(ExtractorConstants.EXTRACT_STATUS, "Success");

        // Check if there were incorrect RICs
        if (!incorrectRICs.isEmpty()) {
            StringBuilder failureReasonSB = new StringBuilder(ExtractorConstants.WARNING_INCORRECT_RICS);
            for (String ric : incorrectRICs) {
                failureReasonSB.append(ric).append(SEPARATOR);
            }
            String failureReason = StringUtils.removeEnd(failureReasonSB.toString(), SEPARATOR);
            staXBuilder.addElement(ExtractorConstants.FAILURE_REASON, failureReason);
        }
        staXBuilder.endElement(); // Close report details element

        // Generate the restricted securities list
        staXBuilder.startElement(ExtractorConstants.RESTRICTED_SECURITY_LIST);
        for (RestrictedSecurity rsec : restrictions) {
            staXBuilder.startElement(ExtractorConstants.RESTRICTED_SECURITY)
                    .addElement(ExtractorConstants.SECURITY_DESCRIPTION, rsec.getSecurityDescription());

            // Loop through each type of security identifier (ISIN, CUSIP, RIC, WPK)
            for (String isin : rsec.getISINS()) {
                RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.ISIN, isin);
            }
            for (String cusip : rsec.getCUSIPs()) {
                RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.CUSIP, cusip);
            }
            for (String ric : rsec.getRICs()) {
                RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.RIC, ric);
            }
            for (String wpk : rsec.getWPKs()) {
                RSReportUtil.addSecurityIdentifier(staXBuilder, ExtractorConstants.WPK, wpk);
            }
            staXBuilder.endElement(); // Close restricted security element
        }
        staXBuilder.endElement(); // Close restricted security list element
    }
}
