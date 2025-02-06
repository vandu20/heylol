package com.db.fusion.rs.util;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;
import org.apache.commons.lang.Validate;

/**
 * Helper for calendar operations
 * @author Alexey Surkov
 */
public final class DateTimeHelper {

    // Set GMT TimeZone
    private static final TimeZone GMT = TimeZone.getTimeZone("GMT");

    // SimpleDateFormat for GMT formatting
    private static SimpleDateFormat dateFormatGmt = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

    static {
        // Set TimeZone for formatting
        dateFormatGmt.setTimeZone(GMT);
    }

    // Private constructor to prevent instantiation
    private DateTimeHelper() {}

    /**
     * Formats Date to GMT
     * @param timestamp Date to be formatted
     * @return String representation in GMT
     */
    public static synchronized String formatDateTimeGmt(Date timestamp) {
        Validate.notNull(timestamp);
        return dateFormatGmt.format(timestamp);
    }

    /**
     * Formats Calendar to GMT
     * @param timestamp Calendar to be formatted
     * @return String representation in GMT
     */
    public static synchronized String formatCalendarGmt(Calendar timestamp) {
        return formatDateTimeGmt(timestamp.getTime());
    }

    /**
     * Get GMT Calendar instance
     * @return GMT Calendar instance
     */
    public static Calendar getGMTCalendar() {
        return Calendar.getInstance(GMT);
    }
}
