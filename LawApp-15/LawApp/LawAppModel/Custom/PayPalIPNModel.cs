﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawAppModel
{
    public enum PayPalPaymentStatus
    {
        //http://stackoverflow.com/questions/2433329/possible-payment-status-values
        Completed, Denied, Expired, Failed, Pending, Refunded, Reversed, Processed, Voided, Other
    }

    public class PayPalIPNModel
    {
        //ipnguide.pdf - page 43
        #region "Transaction and Notification-Related Variables"
        /// use this to verify its not spoofed, this is our info
        public string receiver_email { get; set; } //127
        public string receiver_id { get; set; } //13
        /// Keep this ID to avoid processing the transaction twice
        /// The merchant’s original transaction identification number for the payment from the buyer, against which the case was registered.
        public string txn_id { get; set; }
        /// The kind of transaction for which the IPN message was sent.
        public string txn_type { get; set; }
        /// Encrypted string used to validate the authenticity of the transaction
        public string verify_sign { get; set; }

        public Guid? AppToken { get; set; }
        #endregion

        #region "Buyer Information Variables"

        public string address_country { get; set; } //64
        public string address_city { get; set; } //40
        public string address_country_code { get; set; }  //2
        public string address_name { get; set; } //128 - prob don't need
        public string address_state { get; set; } //40
        public string address_status { get; set; }  //confirmed/unconfirmed
        public string address_street { get; set; } //200
        public string address_zip { get; set; } //20
        public string contact_phone { get; set; } //20
        public string first_name { get; set; } //64
        public string last_name { get; set; } //64
        public string payer_email { get; set; } //127
        public string payer_id { get; set; }  //13

        public int? Zip
        {
            get
            {
                int temp;
                if (int.TryParse(address_zip, out temp))
                {
                    return temp;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
        #region "Payment Information Variables"

        /*
        public string auth_amount { get; set; }
        public string auth_exp { get; set; } //28
        public string auth_id { get; set; } //19
        public string auth_status { get; set; }
         */

        /// Token passed back from PayPal for cross ref
        public string token { get; set; }
        //public string checkoutstatus { get; set; }
        /// Passthrough variable you can use to identify your Invoice Number for this purchase. If omitted, no variable is passed back.
        public string invoice { get; set; } //127
        /// Item name as passed by you, the merchant. Or, if not passed by you, as
        /// entered by your customer. If this is a shopping cart transaction, PayPal
        /// will append the number of the item (e.g., item_name1, item_name2,
        /// and so forth).
        public string item_name1 { get; set; } //127
        public string item_number1 { get; set; } //127
        public string mc_currency { get; set; } //currency of the payment.
        public string mc_fee { get; set; } //Transaction fee associated with the payment
        public string mc_gross { get; set; }    //Full amount of the customer's payment 
        /// Whether the customer has a verified PayPal account.
        /// verified – Customer has a verified PayPal account
        /// unverified – Customer has an unverified PayPal account.
        public string payer_status { get; set; }
        /// HH:MM:SS Mmm DD, YYYY PDT (28chars)
        public string payment_date { get; set; }

        public DateTime TrxnDate
        {
            get
            {
                DateTime dt = DateTime.Now;
                if (DateTime.TryParse(payment_date, out dt))
                {
                    return dt;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

        /// The status of the payment:
        /// Canceled_Reversal: A reversal has been canceled. For example, you
        /// won a dispute with the customer, and the funds for the transaction that was
        /// reversed have been returned to you.
        /// Completed: The payment has been completed, and the funds have been added successfully to your account balance
        ///Created: A German ELV payment is made using Express Checkout. Denied: You denied the payment. This happens only if the payment was
        /// previously pending because of possible reasons described for the pending_reason variable or the Fraud_Management_Filters_x variable.
        /// Expired: This authorization has expired and cannot be captured.
        ///Failed: The payment has failed. This happens only if the payment was made from your customer’s bank account.
        ///Pending: The payment is pending. See pending_reason for more information.
        ///Refunded: You refunded the payment.
        ///Reversed: A payment was reversed due to a chargeback or other type of reversal. The funds have been removed from your account balance and
        ///returned to the buyer. The reason for the reversal is specified in the ReasonCode element.
        ///Processed: A payment has been accepted.
        ///Voided: This authorization has been voided.
        public string payment_status {get;set;}
        public PayPalPaymentStatus? PaymentStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(payment_status))
                {
                    return (PayPalPaymentStatus?)null;
                }

                switch (payment_status)
                {
                    case "Completed": return PayPalPaymentStatus.Completed;
                    case "Denied": return PayPalPaymentStatus.Denied;
                    case "Failed": return PayPalPaymentStatus.Failed;
                    case "Pending": return PayPalPaymentStatus.Pending;
                    case "Refunded": return PayPalPaymentStatus.Refunded;
                    case "Reversed": return PayPalPaymentStatus.Reversed;
                    case "Processed": return PayPalPaymentStatus.Processed;
                    case "Voided": return PayPalPaymentStatus.Voided;
                    default: return PayPalPaymentStatus.Other;
                }
            }
        }


        /// echeck: This payment was funded with an eCheck.
        /// instant: This payment was funded with PayPal balance, credit card, or Instant Transfer.
        public string payment_type { get; set; }
        /// This variable is set only if payment_status = Pending. - too many reasons (look it up in pdf)
        public string pending_reason { get; set; }
        public string protection_eligibility { get; set; }
        public string quantity { get; set; }
        public string reason_code { get; set; }
        public string correlationID { get; set; }
        public string ack { get; set; }
        public string errmsg { get; set; }
        public int? errcode { get; set; }

        /// should hold the clientid passed in from setexpresscheckout
        public string custom { get; set; }

        public decimal Total
        {
            get
            {
                decimal amount = 0;
                if (decimal.TryParse(mc_gross, out amount))
                {
                    return amount;
                }
                else
                {
                    return 0;
                }
            }
        }
        public decimal Fee
        {
            get
            {
                decimal amount = 0;
                if (decimal.TryParse(mc_fee, out amount))
                {
                    return amount;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion



        // Populate
        public PayPalIPNModel(NameValueCollection form)
        {
            int tryInt;

            this.ack = form["ack"];
            this.address_city = form["address_city"];
            this.address_country = form["address_country"];
            this.address_country_code = form["address_country_code"];
            this.address_name = form["address_name"];
            this.address_state = form["address_state"];
            this.address_status = form["address_status"];
            this.address_street = form["address_street"];
            this.address_zip = form["address_zip"];
            this.contact_phone = form["contact_phone"];
            this.correlationID = form["correlationID"];
            this.custom = form["custom"];

            this.errcode = int.TryParse(form["errcode"], out tryInt) ? tryInt : (int?)null;
            this.errmsg = form["errmsg"];
            this.first_name = form["first_name"];
            this.invoice = form["invoice"];
            this.item_name1 = form["item_name1"];
            this.item_number1 = form["item_number1"];
            this.last_name = form["last_name"];
            this.mc_currency = form["mc_currency"];
            this.mc_fee = form["mc_fee"];
            this.mc_gross = form["mc_gross"];

            this.payer_email = form["payer_email"];
            this.payer_id = form["payer_id"];
            this.payer_status = form["payer_status"];
            this.payment_date = form["payment_date"];
            this.payment_status = form["payment_status"];
            this.payment_type = form["payment_type"];
            this.pending_reason = form["pending_reason"];
            this.protection_eligibility = form["protection_eligibility"];
            this.quantity = form["quantity"];
            this.reason_code = form["reason_code"];
            this.receiver_email = form["receiver_email"];
            this.receiver_id = form["receiver_id"];
            this.token = form["token"];
            this.txn_id = form["txn_id"];
            this.txn_type = form["txn_type"];
            this.verify_sign = form["verify_sign"];

            Guid tryGuid;
            this.AppToken = Guid.TryParse(form["custom"], out tryGuid) ? tryGuid : (Guid?)null;
        }
    }
}