/******/ (function (modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if (installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
         /******/
      }
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
         /******/
      };
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
      /******/
   }
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// identity function for calling harmony imports with the correct context
/******/ 	__webpack_require__.i = function (value) { return value; };
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function (exports, name, getter) {
/******/ 		if (!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
            /******/
         });
         /******/
      }
      /******/
   };
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function (module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
      /******/
   };
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function (object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "/dist/";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 52);
   /******/
})
/************************************************************************/
/******/([
/* 0 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
         // Copyright Joyent, Inc. and other Node contributors.
         //
         // Permission is hereby granted, free of charge, to any person obtaining a
         // copy of this software and associated documentation files (the
         // "Software"), to deal in the Software without restriction, including
         // without limitation the rights to use, copy, modify, merge, publish,
         // distribute, sublicense, and/or sell copies of the Software, and to permit
         // persons to whom the Software is furnished to do so, subject to the
         // following conditions:
         //
         // The above copyright notice and this permission notice shall be included
         // in all copies or substantial portions of the Software.
         //
         // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
         // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
         // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
         // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
         // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
         // USE OR OTHER DEALINGS IN THE SOFTWARE.

         // a duplex stream is just a stream that is both readable and writable.
         // Since JS doesn't have multiple prototypal inheritance, this class
         // prototypally inherits from Readable, and then parasitically from
         // Writable.



         /*<replacement>*/

         var processNextTick = __webpack_require__(10);
         /*</replacement>*/

         /*<replacement>*/
         var objectKeys = Object.keys || function (obj) {
            var keys = [];
            for (var key in obj) {
               keys.push(key);
            } return keys;
         };
         /*</replacement>*/

         module.exports = Duplex;

         /*<replacement>*/
         var util = __webpack_require__(5);
         util.inherits = __webpack_require__(2);
         /*</replacement>*/

         var Readable = __webpack_require__(22);
         var Writable = __webpack_require__(24);

         util.inherits(Duplex, Readable);

         var keys = objectKeys(Writable.prototype);
         for (var v = 0; v < keys.length; v++) {
            var method = keys[v];
            if (!Duplex.prototype[method]) Duplex.prototype[method] = Writable.prototype[method];
         }

         function Duplex(options) {
            if (!(this instanceof Duplex)) return new Duplex(options);

            Readable.call(this, options);
            Writable.call(this, options);

            if (options && options.readable === false) this.readable = false;

            if (options && options.writable === false) this.writable = false;

            this.allowHalfOpen = true;
            if (options && options.allowHalfOpen === false) this.allowHalfOpen = false;

            this.once('end', onend);
         }

         // the no-half-open enforcer
         function onend() {
            // if we allow half-open state, or if the writable side ended,
            // then we're ok.
            if (this.allowHalfOpen || this._writableState.ended) return;

            // no more data can be written.
            // But allow more writes to happen in this tick.
            processNextTick(onEndNT, this);
         }

         function onEndNT(self) {
            self.end();
         }

         Object.defineProperty(Duplex.prototype, 'destroyed', {
            get: function get() {
               if (this._readableState === undefined || this._writableState === undefined) {
                  return false;
               }
               return this._readableState.destroyed && this._writableState.destroyed;
            },
            set: function set(value) {
               // we ignore the value if the stream
               // has not been initialized yet
               if (this._readableState === undefined || this._writableState === undefined) {
                  return;
               }

               // backward compatibility, the user is explicitly
               // managing destroyed
               this._readableState.destroyed = value;
               this._writableState.destroyed = value;
            }
         });

         Duplex.prototype._destroy = function (err, cb) {
            this.push(null);
            this.end();

            processNextTick(cb, err);
         };

         function forEach(xs, f) {
            for (var i = 0, l = xs.length; i < l; i++) {
               f(xs[i], i);
            }
         }

         /***/
      }),
/* 1 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

         var g;

         // This works in non-strict mode
         g = function () {
            return this;
         }();

         try {
            // This works if eval is allowed (see CSP)
            g = g || Function("return this")() || (1, eval)("this");
         } catch (e) {
            // This works if the window reference is available
            if ((typeof window === "undefined" ? "undefined" : _typeof(window)) === "object") g = window;
         }

         // g can still be undefined, but nothing to do about it...
         // We return undefined, instead of nothing here, so it's
         // easier to handle this case. if(!global) { ...}

         module.exports = g;

         /***/
      }),
/* 2 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         if (typeof Object.create === 'function') {
            // implementation from standard node.js 'util' module
            module.exports = function inherits(ctor, superCtor) {
               ctor.super_ = superCtor;
               ctor.prototype = Object.create(superCtor.prototype, {
                  constructor: {
                     value: ctor,
                     enumerable: false,
                     writable: true,
                     configurable: true
                  }
               });
            };
         } else {
            // old school shim for old browsers
            module.exports = function inherits(ctor, superCtor) {
               ctor.super_ = superCtor;
               var TempCtor = function TempCtor() { };
               TempCtor.prototype = superCtor.prototype;
               ctor.prototype = new TempCtor();
               ctor.prototype.constructor = ctor;
            };
         }

         /***/
      }),
/* 3 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         Object.defineProperty(exports, "__esModule", { value: true });
         var LogLevel;
         (function (LogLevel) {
            LogLevel[LogLevel["Trace"] = 0] = "Trace";
            LogLevel[LogLevel["Information"] = 1] = "Information";
            LogLevel[LogLevel["Warning"] = 2] = "Warning";
            LogLevel[LogLevel["Error"] = 3] = "Error";
            LogLevel[LogLevel["None"] = 4] = "None";
         })(LogLevel = exports.LogLevel || (exports.LogLevel = {}));

         /***/
      }),
/* 4 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global) {/*!
 * The buffer module from node.js, for the browser.
 *
 * @author   Feross Aboukhadijeh <feross@feross.org> <http://feross.org>
 * @license  MIT
 */
            /* eslint-disable no-proto */



            var base64 = __webpack_require__(34);
            var ieee754 = __webpack_require__(35);
            var isArray = __webpack_require__(20);

            exports.Buffer = Buffer;
            exports.SlowBuffer = SlowBuffer;
            exports.INSPECT_MAX_BYTES = 50;

            /**
             * If `Buffer.TYPED_ARRAY_SUPPORT`:
             *   === true    Use Uint8Array implementation (fastest)
             *   === false   Use Object implementation (most compatible, even IE6)
             *
             * Browsers that support typed arrays are IE 10+, Firefox 4+, Chrome 7+, Safari 5.1+,
             * Opera 11.6+, iOS 4.2+.
             *
             * Due to various browser bugs, sometimes the Object implementation will be used even
             * when the browser supports typed arrays.
             *
             * Note:
             *
             *   - Firefox 4-29 lacks support for adding new properties to `Uint8Array` instances,
             *     See: https://bugzilla.mozilla.org/show_bug.cgi?id=695438.
             *
             *   - Chrome 9-10 is missing the `TypedArray.prototype.subarray` function.
             *
             *   - IE10 has a broken `TypedArray.prototype.subarray` function which returns arrays of
             *     incorrect length in some situations.
            
             * We detect these buggy browsers and set `Buffer.TYPED_ARRAY_SUPPORT` to `false` so they
             * get the Object implementation, which is slower but behaves correctly.
             */
            Buffer.TYPED_ARRAY_SUPPORT = global.TYPED_ARRAY_SUPPORT !== undefined ? global.TYPED_ARRAY_SUPPORT : typedArraySupport();

            /*
             * Export kMaxLength after typed array support is determined.
             */
            exports.kMaxLength = kMaxLength();

            function typedArraySupport() {
               try {
                  var arr = new Uint8Array(1);
                  arr.__proto__ = {
                     __proto__: Uint8Array.prototype, foo: function foo() {
                        return 42;
                     }
                  };
                  return arr.foo() === 42 && // typed array instances can be augmented
                     typeof arr.subarray === 'function' && // chrome 9-10 lack `subarray`
                     arr.subarray(1, 1).byteLength === 0; // ie10 has broken `subarray`
               } catch (e) {
                  return false;
               }
            }

            function kMaxLength() {
               return Buffer.TYPED_ARRAY_SUPPORT ? 0x7fffffff : 0x3fffffff;
            }

            function createBuffer(that, length) {
               if (kMaxLength() < length) {
                  throw new RangeError('Invalid typed array length');
               }
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  // Return an augmented `Uint8Array` instance, for best performance
                  that = new Uint8Array(length);
                  that.__proto__ = Buffer.prototype;
               } else {
                  // Fallback: Return an object instance of the Buffer class
                  if (that === null) {
                     that = new Buffer(length);
                  }
                  that.length = length;
               }

               return that;
            }

            /**
             * The Buffer constructor returns instances of `Uint8Array` that have their
             * prototype changed to `Buffer.prototype`. Furthermore, `Buffer` is a subclass of
             * `Uint8Array`, so the returned instances will have all the node `Buffer` methods
             * and the `Uint8Array` methods. Square bracket notation works as expected -- it
             * returns a single octet.
             *
             * The `Uint8Array` prototype remains unmodified.
             */

            function Buffer(arg, encodingOrOffset, length) {
               if (!Buffer.TYPED_ARRAY_SUPPORT && !(this instanceof Buffer)) {
                  return new Buffer(arg, encodingOrOffset, length);
               }

               // Common case.
               if (typeof arg === 'number') {
                  if (typeof encodingOrOffset === 'string') {
                     throw new Error('If encoding is specified then the first argument must be a string');
                  }
                  return allocUnsafe(this, arg);
               }
               return from(this, arg, encodingOrOffset, length);
            }

            Buffer.poolSize = 8192; // not used by this implementation

            // TODO: Legacy, not needed anymore. Remove in next major version.
            Buffer._augment = function (arr) {
               arr.__proto__ = Buffer.prototype;
               return arr;
            };

            function from(that, value, encodingOrOffset, length) {
               if (typeof value === 'number') {
                  throw new TypeError('"value" argument must not be a number');
               }

               if (typeof ArrayBuffer !== 'undefined' && value instanceof ArrayBuffer) {
                  return fromArrayBuffer(that, value, encodingOrOffset, length);
               }

               if (typeof value === 'string') {
                  return fromString(that, value, encodingOrOffset);
               }

               return fromObject(that, value);
            }

            /**
             * Functionally equivalent to Buffer(arg, encoding) but throws a TypeError
             * if value is a number.
             * Buffer.from(str[, encoding])
             * Buffer.from(array)
             * Buffer.from(buffer)
             * Buffer.from(arrayBuffer[, byteOffset[, length]])
             **/
            Buffer.from = function (value, encodingOrOffset, length) {
               return from(null, value, encodingOrOffset, length);
            };

            if (Buffer.TYPED_ARRAY_SUPPORT) {
               Buffer.prototype.__proto__ = Uint8Array.prototype;
               Buffer.__proto__ = Uint8Array;
               if (typeof Symbol !== 'undefined' && Symbol.species && Buffer[Symbol.species] === Buffer) {
                  // Fix subarray() in ES2016. See: https://github.com/feross/buffer/pull/97
                  Object.defineProperty(Buffer, Symbol.species, {
                     value: null,
                     configurable: true
                  });
               }
            }

            function assertSize(size) {
               if (typeof size !== 'number') {
                  throw new TypeError('"size" argument must be a number');
               } else if (size < 0) {
                  throw new RangeError('"size" argument must not be negative');
               }
            }

            function alloc(that, size, fill, encoding) {
               assertSize(size);
               if (size <= 0) {
                  return createBuffer(that, size);
               }
               if (fill !== undefined) {
                  // Only pay attention to encoding if it's a string. This
                  // prevents accidentally sending in a number that would
                  // be interpretted as a start offset.
                  return typeof encoding === 'string' ? createBuffer(that, size).fill(fill, encoding) : createBuffer(that, size).fill(fill);
               }
               return createBuffer(that, size);
            }

            /**
             * Creates a new filled Buffer instance.
             * alloc(size[, fill[, encoding]])
             **/
            Buffer.alloc = function (size, fill, encoding) {
               return alloc(null, size, fill, encoding);
            };

            function allocUnsafe(that, size) {
               assertSize(size);
               that = createBuffer(that, size < 0 ? 0 : checked(size) | 0);
               if (!Buffer.TYPED_ARRAY_SUPPORT) {
                  for (var i = 0; i < size; ++i) {
                     that[i] = 0;
                  }
               }
               return that;
            }

            /**
             * Equivalent to Buffer(num), by default creates a non-zero-filled Buffer instance.
             * */
            Buffer.allocUnsafe = function (size) {
               return allocUnsafe(null, size);
            };
            /**
             * Equivalent to SlowBuffer(num), by default creates a non-zero-filled Buffer instance.
             */
            Buffer.allocUnsafeSlow = function (size) {
               return allocUnsafe(null, size);
            };

            function fromString(that, string, encoding) {
               if (typeof encoding !== 'string' || encoding === '') {
                  encoding = 'utf8';
               }

               if (!Buffer.isEncoding(encoding)) {
                  throw new TypeError('"encoding" must be a valid string encoding');
               }

               var length = byteLength(string, encoding) | 0;
               that = createBuffer(that, length);

               var actual = that.write(string, encoding);

               if (actual !== length) {
                  // Writing a hex string, for example, that contains invalid characters will
                  // cause everything after the first invalid character to be ignored. (e.g.
                  // 'abxxcd' will be treated as 'ab')
                  that = that.slice(0, actual);
               }

               return that;
            }

            function fromArrayLike(that, array) {
               var length = array.length < 0 ? 0 : checked(array.length) | 0;
               that = createBuffer(that, length);
               for (var i = 0; i < length; i += 1) {
                  that[i] = array[i] & 255;
               }
               return that;
            }

            function fromArrayBuffer(that, array, byteOffset, length) {
               array.byteLength; // this throws if `array` is not a valid ArrayBuffer

               if (byteOffset < 0 || array.byteLength < byteOffset) {
                  throw new RangeError('\'offset\' is out of bounds');
               }

               if (array.byteLength < byteOffset + (length || 0)) {
                  throw new RangeError('\'length\' is out of bounds');
               }

               if (byteOffset === undefined && length === undefined) {
                  array = new Uint8Array(array);
               } else if (length === undefined) {
                  array = new Uint8Array(array, byteOffset);
               } else {
                  array = new Uint8Array(array, byteOffset, length);
               }

               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  // Return an augmented `Uint8Array` instance, for best performance
                  that = array;
                  that.__proto__ = Buffer.prototype;
               } else {
                  // Fallback: Return an object instance of the Buffer class
                  that = fromArrayLike(that, array);
               }
               return that;
            }

            function fromObject(that, obj) {
               if (Buffer.isBuffer(obj)) {
                  var len = checked(obj.length) | 0;
                  that = createBuffer(that, len);

                  if (that.length === 0) {
                     return that;
                  }

                  obj.copy(that, 0, 0, len);
                  return that;
               }

               if (obj) {
                  if (typeof ArrayBuffer !== 'undefined' && obj.buffer instanceof ArrayBuffer || 'length' in obj) {
                     if (typeof obj.length !== 'number' || isnan(obj.length)) {
                        return createBuffer(that, 0);
                     }
                     return fromArrayLike(that, obj);
                  }

                  if (obj.type === 'Buffer' && isArray(obj.data)) {
                     return fromArrayLike(that, obj.data);
                  }
               }

               throw new TypeError('First argument must be a string, Buffer, ArrayBuffer, Array, or array-like object.');
            }

            function checked(length) {
               // Note: cannot use `length < kMaxLength()` here because that fails when
               // length is NaN (which is otherwise coerced to zero.)
               if (length >= kMaxLength()) {
                  throw new RangeError('Attempt to allocate Buffer larger than maximum ' + 'size: 0x' + kMaxLength().toString(16) + ' bytes');
               }
               return length | 0;
            }

            function SlowBuffer(length) {
               if (+length != length) {
                  // eslint-disable-line eqeqeq
                  length = 0;
               }
               return Buffer.alloc(+length);
            }

            Buffer.isBuffer = function isBuffer(b) {
               return !!(b != null && b._isBuffer);
            };

            Buffer.compare = function compare(a, b) {
               if (!Buffer.isBuffer(a) || !Buffer.isBuffer(b)) {
                  throw new TypeError('Arguments must be Buffers');
               }

               if (a === b) return 0;

               var x = a.length;
               var y = b.length;

               for (var i = 0, len = Math.min(x, y); i < len; ++i) {
                  if (a[i] !== b[i]) {
                     x = a[i];
                     y = b[i];
                     break;
                  }
               }

               if (x < y) return -1;
               if (y < x) return 1;
               return 0;
            };

            Buffer.isEncoding = function isEncoding(encoding) {
               switch (String(encoding).toLowerCase()) {
                  case 'hex':
                  case 'utf8':
                  case 'utf-8':
                  case 'ascii':
                  case 'latin1':
                  case 'binary':
                  case 'base64':
                  case 'ucs2':
                  case 'ucs-2':
                  case 'utf16le':
                  case 'utf-16le':
                     return true;
                  default:
                     return false;
               }
            };

            Buffer.concat = function concat(list, length) {
               if (!isArray(list)) {
                  throw new TypeError('"list" argument must be an Array of Buffers');
               }

               if (list.length === 0) {
                  return Buffer.alloc(0);
               }

               var i;
               if (length === undefined) {
                  length = 0;
                  for (i = 0; i < list.length; ++i) {
                     length += list[i].length;
                  }
               }

               var buffer = Buffer.allocUnsafe(length);
               var pos = 0;
               for (i = 0; i < list.length; ++i) {
                  var buf = list[i];
                  if (!Buffer.isBuffer(buf)) {
                     throw new TypeError('"list" argument must be an Array of Buffers');
                  }
                  buf.copy(buffer, pos);
                  pos += buf.length;
               }
               return buffer;
            };

            function byteLength(string, encoding) {
               if (Buffer.isBuffer(string)) {
                  return string.length;
               }
               if (typeof ArrayBuffer !== 'undefined' && typeof ArrayBuffer.isView === 'function' && (ArrayBuffer.isView(string) || string instanceof ArrayBuffer)) {
                  return string.byteLength;
               }
               if (typeof string !== 'string') {
                  string = '' + string;
               }

               var len = string.length;
               if (len === 0) return 0;

               // Use a for loop to avoid recursion
               var loweredCase = false;
               for (; ;) {
                  switch (encoding) {
                     case 'ascii':
                     case 'latin1':
                     case 'binary':
                        return len;
                     case 'utf8':
                     case 'utf-8':
                     case undefined:
                        return utf8ToBytes(string).length;
                     case 'ucs2':
                     case 'ucs-2':
                     case 'utf16le':
                     case 'utf-16le':
                        return len * 2;
                     case 'hex':
                        return len >>> 1;
                     case 'base64':
                        return base64ToBytes(string).length;
                     default:
                        if (loweredCase) return utf8ToBytes(string).length; // assume utf8
                        encoding = ('' + encoding).toLowerCase();
                        loweredCase = true;
                  }
               }
            }
            Buffer.byteLength = byteLength;

            function slowToString(encoding, start, end) {
               var loweredCase = false;

               // No need to verify that "this.length <= MAX_UINT32" since it's a read-only
               // property of a typed array.

               // This behaves neither like String nor Uint8Array in that we set start/end
               // to their upper/lower bounds if the value passed is out of range.
               // undefined is handled specially as per ECMA-262 6th Edition,
               // Section 13.3.3.7 Runtime Semantics: KeyedBindingInitialization.
               if (start === undefined || start < 0) {
                  start = 0;
               }
               // Return early if start > this.length. Done here to prevent potential uint32
               // coercion fail below.
               if (start > this.length) {
                  return '';
               }

               if (end === undefined || end > this.length) {
                  end = this.length;
               }

               if (end <= 0) {
                  return '';
               }

               // Force coersion to uint32. This will also coerce falsey/NaN values to 0.
               end >>>= 0;
               start >>>= 0;

               if (end <= start) {
                  return '';
               }

               if (!encoding) encoding = 'utf8';

               while (true) {
                  switch (encoding) {
                     case 'hex':
                        return hexSlice(this, start, end);

                     case 'utf8':
                     case 'utf-8':
                        return utf8Slice(this, start, end);

                     case 'ascii':
                        return asciiSlice(this, start, end);

                     case 'latin1':
                     case 'binary':
                        return latin1Slice(this, start, end);

                     case 'base64':
                        return base64Slice(this, start, end);

                     case 'ucs2':
                     case 'ucs-2':
                     case 'utf16le':
                     case 'utf-16le':
                        return utf16leSlice(this, start, end);

                     default:
                        if (loweredCase) throw new TypeError('Unknown encoding: ' + encoding);
                        encoding = (encoding + '').toLowerCase();
                        loweredCase = true;
                  }
               }
            }

            // The property is used by `Buffer.isBuffer` and `is-buffer` (in Safari 5-7) to detect
            // Buffer instances.
            Buffer.prototype._isBuffer = true;

            function swap(b, n, m) {
               var i = b[n];
               b[n] = b[m];
               b[m] = i;
            }

            Buffer.prototype.swap16 = function swap16() {
               var len = this.length;
               if (len % 2 !== 0) {
                  throw new RangeError('Buffer size must be a multiple of 16-bits');
               }
               for (var i = 0; i < len; i += 2) {
                  swap(this, i, i + 1);
               }
               return this;
            };

            Buffer.prototype.swap32 = function swap32() {
               var len = this.length;
               if (len % 4 !== 0) {
                  throw new RangeError('Buffer size must be a multiple of 32-bits');
               }
               for (var i = 0; i < len; i += 4) {
                  swap(this, i, i + 3);
                  swap(this, i + 1, i + 2);
               }
               return this;
            };

            Buffer.prototype.swap64 = function swap64() {
               var len = this.length;
               if (len % 8 !== 0) {
                  throw new RangeError('Buffer size must be a multiple of 64-bits');
               }
               for (var i = 0; i < len; i += 8) {
                  swap(this, i, i + 7);
                  swap(this, i + 1, i + 6);
                  swap(this, i + 2, i + 5);
                  swap(this, i + 3, i + 4);
               }
               return this;
            };

            Buffer.prototype.toString = function toString() {
               var length = this.length | 0;
               if (length === 0) return '';
               if (arguments.length === 0) return utf8Slice(this, 0, length);
               return slowToString.apply(this, arguments);
            };

            Buffer.prototype.equals = function equals(b) {
               if (!Buffer.isBuffer(b)) throw new TypeError('Argument must be a Buffer');
               if (this === b) return true;
               return Buffer.compare(this, b) === 0;
            };

            Buffer.prototype.inspect = function inspect() {
               var str = '';
               var max = exports.INSPECT_MAX_BYTES;
               if (this.length > 0) {
                  str = this.toString('hex', 0, max).match(/.{2}/g).join(' ');
                  if (this.length > max) str += ' ... ';
               }
               return '<Buffer ' + str + '>';
            };

            Buffer.prototype.compare = function compare(target, start, end, thisStart, thisEnd) {
               if (!Buffer.isBuffer(target)) {
                  throw new TypeError('Argument must be a Buffer');
               }

               if (start === undefined) {
                  start = 0;
               }
               if (end === undefined) {
                  end = target ? target.length : 0;
               }
               if (thisStart === undefined) {
                  thisStart = 0;
               }
               if (thisEnd === undefined) {
                  thisEnd = this.length;
               }

               if (start < 0 || end > target.length || thisStart < 0 || thisEnd > this.length) {
                  throw new RangeError('out of range index');
               }

               if (thisStart >= thisEnd && start >= end) {
                  return 0;
               }
               if (thisStart >= thisEnd) {
                  return -1;
               }
               if (start >= end) {
                  return 1;
               }

               start >>>= 0;
               end >>>= 0;
               thisStart >>>= 0;
               thisEnd >>>= 0;

               if (this === target) return 0;

               var x = thisEnd - thisStart;
               var y = end - start;
               var len = Math.min(x, y);

               var thisCopy = this.slice(thisStart, thisEnd);
               var targetCopy = target.slice(start, end);

               for (var i = 0; i < len; ++i) {
                  if (thisCopy[i] !== targetCopy[i]) {
                     x = thisCopy[i];
                     y = targetCopy[i];
                     break;
                  }
               }

               if (x < y) return -1;
               if (y < x) return 1;
               return 0;
            };

            // Finds either the first index of `val` in `buffer` at offset >= `byteOffset`,
            // OR the last index of `val` in `buffer` at offset <= `byteOffset`.
            //
            // Arguments:
            // - buffer - a Buffer to search
            // - val - a string, Buffer, or number
            // - byteOffset - an index into `buffer`; will be clamped to an int32
            // - encoding - an optional encoding, relevant is val is a string
            // - dir - true for indexOf, false for lastIndexOf
            function bidirectionalIndexOf(buffer, val, byteOffset, encoding, dir) {
               // Empty buffer means no match
               if (buffer.length === 0) return -1;

               // Normalize byteOffset
               if (typeof byteOffset === 'string') {
                  encoding = byteOffset;
                  byteOffset = 0;
               } else if (byteOffset > 0x7fffffff) {
                  byteOffset = 0x7fffffff;
               } else if (byteOffset < -0x80000000) {
                  byteOffset = -0x80000000;
               }
               byteOffset = +byteOffset; // Coerce to Number.
               if (isNaN(byteOffset)) {
                  // byteOffset: it it's undefined, null, NaN, "foo", etc, search whole buffer
                  byteOffset = dir ? 0 : buffer.length - 1;
               }

               // Normalize byteOffset: negative offsets start from the end of the buffer
               if (byteOffset < 0) byteOffset = buffer.length + byteOffset;
               if (byteOffset >= buffer.length) {
                  if (dir) return -1; else byteOffset = buffer.length - 1;
               } else if (byteOffset < 0) {
                  if (dir) byteOffset = 0; else return -1;
               }

               // Normalize val
               if (typeof val === 'string') {
                  val = Buffer.from(val, encoding);
               }

               // Finally, search either indexOf (if dir is true) or lastIndexOf
               if (Buffer.isBuffer(val)) {
                  // Special case: looking for empty string/buffer always fails
                  if (val.length === 0) {
                     return -1;
                  }
                  return arrayIndexOf(buffer, val, byteOffset, encoding, dir);
               } else if (typeof val === 'number') {
                  val = val & 0xFF; // Search for a byte value [0-255]
                  if (Buffer.TYPED_ARRAY_SUPPORT && typeof Uint8Array.prototype.indexOf === 'function') {
                     if (dir) {
                        return Uint8Array.prototype.indexOf.call(buffer, val, byteOffset);
                     } else {
                        return Uint8Array.prototype.lastIndexOf.call(buffer, val, byteOffset);
                     }
                  }
                  return arrayIndexOf(buffer, [val], byteOffset, encoding, dir);
               }

               throw new TypeError('val must be string, number or Buffer');
            }

            function arrayIndexOf(arr, val, byteOffset, encoding, dir) {
               var indexSize = 1;
               var arrLength = arr.length;
               var valLength = val.length;

               if (encoding !== undefined) {
                  encoding = String(encoding).toLowerCase();
                  if (encoding === 'ucs2' || encoding === 'ucs-2' || encoding === 'utf16le' || encoding === 'utf-16le') {
                     if (arr.length < 2 || val.length < 2) {
                        return -1;
                     }
                     indexSize = 2;
                     arrLength /= 2;
                     valLength /= 2;
                     byteOffset /= 2;
                  }
               }

               function read(buf, i) {
                  if (indexSize === 1) {
                     return buf[i];
                  } else {
                     return buf.readUInt16BE(i * indexSize);
                  }
               }

               var i;
               if (dir) {
                  var foundIndex = -1;
                  for (i = byteOffset; i < arrLength; i++) {
                     if (read(arr, i) === read(val, foundIndex === -1 ? 0 : i - foundIndex)) {
                        if (foundIndex === -1) foundIndex = i;
                        if (i - foundIndex + 1 === valLength) return foundIndex * indexSize;
                     } else {
                        if (foundIndex !== -1) i -= i - foundIndex;
                        foundIndex = -1;
                     }
                  }
               } else {
                  if (byteOffset + valLength > arrLength) byteOffset = arrLength - valLength;
                  for (i = byteOffset; i >= 0; i--) {
                     var found = true;
                     for (var j = 0; j < valLength; j++) {
                        if (read(arr, i + j) !== read(val, j)) {
                           found = false;
                           break;
                        }
                     }
                     if (found) return i;
                  }
               }

               return -1;
            }

            Buffer.prototype.includes = function includes(val, byteOffset, encoding) {
               return this.indexOf(val, byteOffset, encoding) !== -1;
            };

            Buffer.prototype.indexOf = function indexOf(val, byteOffset, encoding) {
               return bidirectionalIndexOf(this, val, byteOffset, encoding, true);
            };

            Buffer.prototype.lastIndexOf = function lastIndexOf(val, byteOffset, encoding) {
               return bidirectionalIndexOf(this, val, byteOffset, encoding, false);
            };

            function hexWrite(buf, string, offset, length) {
               offset = Number(offset) || 0;
               var remaining = buf.length - offset;
               if (!length) {
                  length = remaining;
               } else {
                  length = Number(length);
                  if (length > remaining) {
                     length = remaining;
                  }
               }

               // must be an even number of digits
               var strLen = string.length;
               if (strLen % 2 !== 0) throw new TypeError('Invalid hex string');

               if (length > strLen / 2) {
                  length = strLen / 2;
               }
               for (var i = 0; i < length; ++i) {
                  var parsed = parseInt(string.substr(i * 2, 2), 16);
                  if (isNaN(parsed)) return i;
                  buf[offset + i] = parsed;
               }
               return i;
            }

            function utf8Write(buf, string, offset, length) {
               return blitBuffer(utf8ToBytes(string, buf.length - offset), buf, offset, length);
            }

            function asciiWrite(buf, string, offset, length) {
               return blitBuffer(asciiToBytes(string), buf, offset, length);
            }

            function latin1Write(buf, string, offset, length) {
               return asciiWrite(buf, string, offset, length);
            }

            function base64Write(buf, string, offset, length) {
               return blitBuffer(base64ToBytes(string), buf, offset, length);
            }

            function ucs2Write(buf, string, offset, length) {
               return blitBuffer(utf16leToBytes(string, buf.length - offset), buf, offset, length);
            }

            Buffer.prototype.write = function write(string, offset, length, encoding) {
               // Buffer#write(string)
               if (offset === undefined) {
                  encoding = 'utf8';
                  length = this.length;
                  offset = 0;
                  // Buffer#write(string, encoding)
               } else if (length === undefined && typeof offset === 'string') {
                  encoding = offset;
                  length = this.length;
                  offset = 0;
                  // Buffer#write(string, offset[, length][, encoding])
               } else if (isFinite(offset)) {
                  offset = offset | 0;
                  if (isFinite(length)) {
                     length = length | 0;
                     if (encoding === undefined) encoding = 'utf8';
                  } else {
                     encoding = length;
                     length = undefined;
                  }
                  // legacy write(string, encoding, offset, length) - remove in v0.13
               } else {
                  throw new Error('Buffer.write(string, encoding, offset[, length]) is no longer supported');
               }

               var remaining = this.length - offset;
               if (length === undefined || length > remaining) length = remaining;

               if (string.length > 0 && (length < 0 || offset < 0) || offset > this.length) {
                  throw new RangeError('Attempt to write outside buffer bounds');
               }

               if (!encoding) encoding = 'utf8';

               var loweredCase = false;
               for (; ;) {
                  switch (encoding) {
                     case 'hex':
                        return hexWrite(this, string, offset, length);

                     case 'utf8':
                     case 'utf-8':
                        return utf8Write(this, string, offset, length);

                     case 'ascii':
                        return asciiWrite(this, string, offset, length);

                     case 'latin1':
                     case 'binary':
                        return latin1Write(this, string, offset, length);

                     case 'base64':
                        // Warning: maxLength not taken into account in base64Write
                        return base64Write(this, string, offset, length);

                     case 'ucs2':
                     case 'ucs-2':
                     case 'utf16le':
                     case 'utf-16le':
                        return ucs2Write(this, string, offset, length);

                     default:
                        if (loweredCase) throw new TypeError('Unknown encoding: ' + encoding);
                        encoding = ('' + encoding).toLowerCase();
                        loweredCase = true;
                  }
               }
            };

            Buffer.prototype.toJSON = function toJSON() {
               return {
                  type: 'Buffer',
                  data: Array.prototype.slice.call(this._arr || this, 0)
               };
            };

            function base64Slice(buf, start, end) {
               if (start === 0 && end === buf.length) {
                  return base64.fromByteArray(buf);
               } else {
                  return base64.fromByteArray(buf.slice(start, end));
               }
            }

            function utf8Slice(buf, start, end) {
               end = Math.min(buf.length, end);
               var res = [];

               var i = start;
               while (i < end) {
                  var firstByte = buf[i];
                  var codePoint = null;
                  var bytesPerSequence = firstByte > 0xEF ? 4 : firstByte > 0xDF ? 3 : firstByte > 0xBF ? 2 : 1;

                  if (i + bytesPerSequence <= end) {
                     var secondByte, thirdByte, fourthByte, tempCodePoint;

                     switch (bytesPerSequence) {
                        case 1:
                           if (firstByte < 0x80) {
                              codePoint = firstByte;
                           }
                           break;
                        case 2:
                           secondByte = buf[i + 1];
                           if ((secondByte & 0xC0) === 0x80) {
                              tempCodePoint = (firstByte & 0x1F) << 0x6 | secondByte & 0x3F;
                              if (tempCodePoint > 0x7F) {
                                 codePoint = tempCodePoint;
                              }
                           }
                           break;
                        case 3:
                           secondByte = buf[i + 1];
                           thirdByte = buf[i + 2];
                           if ((secondByte & 0xC0) === 0x80 && (thirdByte & 0xC0) === 0x80) {
                              tempCodePoint = (firstByte & 0xF) << 0xC | (secondByte & 0x3F) << 0x6 | thirdByte & 0x3F;
                              if (tempCodePoint > 0x7FF && (tempCodePoint < 0xD800 || tempCodePoint > 0xDFFF)) {
                                 codePoint = tempCodePoint;
                              }
                           }
                           break;
                        case 4:
                           secondByte = buf[i + 1];
                           thirdByte = buf[i + 2];
                           fourthByte = buf[i + 3];
                           if ((secondByte & 0xC0) === 0x80 && (thirdByte & 0xC0) === 0x80 && (fourthByte & 0xC0) === 0x80) {
                              tempCodePoint = (firstByte & 0xF) << 0x12 | (secondByte & 0x3F) << 0xC | (thirdByte & 0x3F) << 0x6 | fourthByte & 0x3F;
                              if (tempCodePoint > 0xFFFF && tempCodePoint < 0x110000) {
                                 codePoint = tempCodePoint;
                              }
                           }
                     }
                  }

                  if (codePoint === null) {
                     // we did not generate a valid codePoint so insert a
                     // replacement char (U+FFFD) and advance only 1 byte
                     codePoint = 0xFFFD;
                     bytesPerSequence = 1;
                  } else if (codePoint > 0xFFFF) {
                     // encode to utf16 (surrogate pair dance)
                     codePoint -= 0x10000;
                     res.push(codePoint >>> 10 & 0x3FF | 0xD800);
                     codePoint = 0xDC00 | codePoint & 0x3FF;
                  }

                  res.push(codePoint);
                  i += bytesPerSequence;
               }

               return decodeCodePointsArray(res);
            }

            // Based on http://stackoverflow.com/a/22747272/680742, the browser with
            // the lowest limit is Chrome, with 0x10000 args.
            // We go 1 magnitude less, for safety
            var MAX_ARGUMENTS_LENGTH = 0x1000;

            function decodeCodePointsArray(codePoints) {
               var len = codePoints.length;
               if (len <= MAX_ARGUMENTS_LENGTH) {
                  return String.fromCharCode.apply(String, codePoints); // avoid extra slice()
               }

               // Decode in chunks to avoid "call stack size exceeded".
               var res = '';
               var i = 0;
               while (i < len) {
                  res += String.fromCharCode.apply(String, codePoints.slice(i, i += MAX_ARGUMENTS_LENGTH));
               }
               return res;
            }

            function asciiSlice(buf, start, end) {
               var ret = '';
               end = Math.min(buf.length, end);

               for (var i = start; i < end; ++i) {
                  ret += String.fromCharCode(buf[i] & 0x7F);
               }
               return ret;
            }

            function latin1Slice(buf, start, end) {
               var ret = '';
               end = Math.min(buf.length, end);

               for (var i = start; i < end; ++i) {
                  ret += String.fromCharCode(buf[i]);
               }
               return ret;
            }

            function hexSlice(buf, start, end) {
               var len = buf.length;

               if (!start || start < 0) start = 0;
               if (!end || end < 0 || end > len) end = len;

               var out = '';
               for (var i = start; i < end; ++i) {
                  out += toHex(buf[i]);
               }
               return out;
            }

            function utf16leSlice(buf, start, end) {
               var bytes = buf.slice(start, end);
               var res = '';
               for (var i = 0; i < bytes.length; i += 2) {
                  res += String.fromCharCode(bytes[i] + bytes[i + 1] * 256);
               }
               return res;
            }

            Buffer.prototype.slice = function slice(start, end) {
               var len = this.length;
               start = ~~start;
               end = end === undefined ? len : ~~end;

               if (start < 0) {
                  start += len;
                  if (start < 0) start = 0;
               } else if (start > len) {
                  start = len;
               }

               if (end < 0) {
                  end += len;
                  if (end < 0) end = 0;
               } else if (end > len) {
                  end = len;
               }

               if (end < start) end = start;

               var newBuf;
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  newBuf = this.subarray(start, end);
                  newBuf.__proto__ = Buffer.prototype;
               } else {
                  var sliceLen = end - start;
                  newBuf = new Buffer(sliceLen, undefined);
                  for (var i = 0; i < sliceLen; ++i) {
                     newBuf[i] = this[i + start];
                  }
               }

               return newBuf;
            };

            /*
             * Need to make sure that buffer isn't trying to write out of bounds.
             */
            function checkOffset(offset, ext, length) {
               if (offset % 1 !== 0 || offset < 0) throw new RangeError('offset is not uint');
               if (offset + ext > length) throw new RangeError('Trying to access beyond buffer length');
            }

            Buffer.prototype.readUIntLE = function readUIntLE(offset, byteLength, noAssert) {
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) checkOffset(offset, byteLength, this.length);

               var val = this[offset];
               var mul = 1;
               var i = 0;
               while (++i < byteLength && (mul *= 0x100)) {
                  val += this[offset + i] * mul;
               }

               return val;
            };

            Buffer.prototype.readUIntBE = function readUIntBE(offset, byteLength, noAssert) {
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) {
                  checkOffset(offset, byteLength, this.length);
               }

               var val = this[offset + --byteLength];
               var mul = 1;
               while (byteLength > 0 && (mul *= 0x100)) {
                  val += this[offset + --byteLength] * mul;
               }

               return val;
            };

            Buffer.prototype.readUInt8 = function readUInt8(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 1, this.length);
               return this[offset];
            };

            Buffer.prototype.readUInt16LE = function readUInt16LE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 2, this.length);
               return this[offset] | this[offset + 1] << 8;
            };

            Buffer.prototype.readUInt16BE = function readUInt16BE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 2, this.length);
               return this[offset] << 8 | this[offset + 1];
            };

            Buffer.prototype.readUInt32LE = function readUInt32LE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);

               return (this[offset] | this[offset + 1] << 8 | this[offset + 2] << 16) + this[offset + 3] * 0x1000000;
            };

            Buffer.prototype.readUInt32BE = function readUInt32BE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);

               return this[offset] * 0x1000000 + (this[offset + 1] << 16 | this[offset + 2] << 8 | this[offset + 3]);
            };

            Buffer.prototype.readIntLE = function readIntLE(offset, byteLength, noAssert) {
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) checkOffset(offset, byteLength, this.length);

               var val = this[offset];
               var mul = 1;
               var i = 0;
               while (++i < byteLength && (mul *= 0x100)) {
                  val += this[offset + i] * mul;
               }
               mul *= 0x80;

               if (val >= mul) val -= Math.pow(2, 8 * byteLength);

               return val;
            };

            Buffer.prototype.readIntBE = function readIntBE(offset, byteLength, noAssert) {
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) checkOffset(offset, byteLength, this.length);

               var i = byteLength;
               var mul = 1;
               var val = this[offset + --i];
               while (i > 0 && (mul *= 0x100)) {
                  val += this[offset + --i] * mul;
               }
               mul *= 0x80;

               if (val >= mul) val -= Math.pow(2, 8 * byteLength);

               return val;
            };

            Buffer.prototype.readInt8 = function readInt8(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 1, this.length);
               if (!(this[offset] & 0x80)) return this[offset];
               return (0xff - this[offset] + 1) * -1;
            };

            Buffer.prototype.readInt16LE = function readInt16LE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 2, this.length);
               var val = this[offset] | this[offset + 1] << 8;
               return val & 0x8000 ? val | 0xFFFF0000 : val;
            };

            Buffer.prototype.readInt16BE = function readInt16BE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 2, this.length);
               var val = this[offset + 1] | this[offset] << 8;
               return val & 0x8000 ? val | 0xFFFF0000 : val;
            };

            Buffer.prototype.readInt32LE = function readInt32LE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);

               return this[offset] | this[offset + 1] << 8 | this[offset + 2] << 16 | this[offset + 3] << 24;
            };

            Buffer.prototype.readInt32BE = function readInt32BE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);

               return this[offset] << 24 | this[offset + 1] << 16 | this[offset + 2] << 8 | this[offset + 3];
            };

            Buffer.prototype.readFloatLE = function readFloatLE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);
               return ieee754.read(this, offset, true, 23, 4);
            };

            Buffer.prototype.readFloatBE = function readFloatBE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 4, this.length);
               return ieee754.read(this, offset, false, 23, 4);
            };

            Buffer.prototype.readDoubleLE = function readDoubleLE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 8, this.length);
               return ieee754.read(this, offset, true, 52, 8);
            };

            Buffer.prototype.readDoubleBE = function readDoubleBE(offset, noAssert) {
               if (!noAssert) checkOffset(offset, 8, this.length);
               return ieee754.read(this, offset, false, 52, 8);
            };

            function checkInt(buf, value, offset, ext, max, min) {
               if (!Buffer.isBuffer(buf)) throw new TypeError('"buffer" argument must be a Buffer instance');
               if (value > max || value < min) throw new RangeError('"value" argument is out of bounds');
               if (offset + ext > buf.length) throw new RangeError('Index out of range');
            }

            Buffer.prototype.writeUIntLE = function writeUIntLE(value, offset, byteLength, noAssert) {
               value = +value;
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) {
                  var maxBytes = Math.pow(2, 8 * byteLength) - 1;
                  checkInt(this, value, offset, byteLength, maxBytes, 0);
               }

               var mul = 1;
               var i = 0;
               this[offset] = value & 0xFF;
               while (++i < byteLength && (mul *= 0x100)) {
                  this[offset + i] = value / mul & 0xFF;
               }

               return offset + byteLength;
            };

            Buffer.prototype.writeUIntBE = function writeUIntBE(value, offset, byteLength, noAssert) {
               value = +value;
               offset = offset | 0;
               byteLength = byteLength | 0;
               if (!noAssert) {
                  var maxBytes = Math.pow(2, 8 * byteLength) - 1;
                  checkInt(this, value, offset, byteLength, maxBytes, 0);
               }

               var i = byteLength - 1;
               var mul = 1;
               this[offset + i] = value & 0xFF;
               while (--i >= 0 && (mul *= 0x100)) {
                  this[offset + i] = value / mul & 0xFF;
               }

               return offset + byteLength;
            };

            Buffer.prototype.writeUInt8 = function writeUInt8(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 1, 0xff, 0);
               if (!Buffer.TYPED_ARRAY_SUPPORT) value = Math.floor(value);
               this[offset] = value & 0xff;
               return offset + 1;
            };

            function objectWriteUInt16(buf, value, offset, littleEndian) {
               if (value < 0) value = 0xffff + value + 1;
               for (var i = 0, j = Math.min(buf.length - offset, 2); i < j; ++i) {
                  buf[offset + i] = (value & 0xff << 8 * (littleEndian ? i : 1 - i)) >>> (littleEndian ? i : 1 - i) * 8;
               }
            }

            Buffer.prototype.writeUInt16LE = function writeUInt16LE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 2, 0xffff, 0);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value & 0xff;
                  this[offset + 1] = value >>> 8;
               } else {
                  objectWriteUInt16(this, value, offset, true);
               }
               return offset + 2;
            };

            Buffer.prototype.writeUInt16BE = function writeUInt16BE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 2, 0xffff, 0);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value >>> 8;
                  this[offset + 1] = value & 0xff;
               } else {
                  objectWriteUInt16(this, value, offset, false);
               }
               return offset + 2;
            };

            function objectWriteUInt32(buf, value, offset, littleEndian) {
               if (value < 0) value = 0xffffffff + value + 1;
               for (var i = 0, j = Math.min(buf.length - offset, 4); i < j; ++i) {
                  buf[offset + i] = value >>> (littleEndian ? i : 3 - i) * 8 & 0xff;
               }
            }

            Buffer.prototype.writeUInt32LE = function writeUInt32LE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 4, 0xffffffff, 0);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset + 3] = value >>> 24;
                  this[offset + 2] = value >>> 16;
                  this[offset + 1] = value >>> 8;
                  this[offset] = value & 0xff;
               } else {
                  objectWriteUInt32(this, value, offset, true);
               }
               return offset + 4;
            };

            Buffer.prototype.writeUInt32BE = function writeUInt32BE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 4, 0xffffffff, 0);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value >>> 24;
                  this[offset + 1] = value >>> 16;
                  this[offset + 2] = value >>> 8;
                  this[offset + 3] = value & 0xff;
               } else {
                  objectWriteUInt32(this, value, offset, false);
               }
               return offset + 4;
            };

            Buffer.prototype.writeIntLE = function writeIntLE(value, offset, byteLength, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) {
                  var limit = Math.pow(2, 8 * byteLength - 1);

                  checkInt(this, value, offset, byteLength, limit - 1, -limit);
               }

               var i = 0;
               var mul = 1;
               var sub = 0;
               this[offset] = value & 0xFF;
               while (++i < byteLength && (mul *= 0x100)) {
                  if (value < 0 && sub === 0 && this[offset + i - 1] !== 0) {
                     sub = 1;
                  }
                  this[offset + i] = (value / mul >> 0) - sub & 0xFF;
               }

               return offset + byteLength;
            };

            Buffer.prototype.writeIntBE = function writeIntBE(value, offset, byteLength, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) {
                  var limit = Math.pow(2, 8 * byteLength - 1);

                  checkInt(this, value, offset, byteLength, limit - 1, -limit);
               }

               var i = byteLength - 1;
               var mul = 1;
               var sub = 0;
               this[offset + i] = value & 0xFF;
               while (--i >= 0 && (mul *= 0x100)) {
                  if (value < 0 && sub === 0 && this[offset + i + 1] !== 0) {
                     sub = 1;
                  }
                  this[offset + i] = (value / mul >> 0) - sub & 0xFF;
               }

               return offset + byteLength;
            };

            Buffer.prototype.writeInt8 = function writeInt8(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 1, 0x7f, -0x80);
               if (!Buffer.TYPED_ARRAY_SUPPORT) value = Math.floor(value);
               if (value < 0) value = 0xff + value + 1;
               this[offset] = value & 0xff;
               return offset + 1;
            };

            Buffer.prototype.writeInt16LE = function writeInt16LE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 2, 0x7fff, -0x8000);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value & 0xff;
                  this[offset + 1] = value >>> 8;
               } else {
                  objectWriteUInt16(this, value, offset, true);
               }
               return offset + 2;
            };

            Buffer.prototype.writeInt16BE = function writeInt16BE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 2, 0x7fff, -0x8000);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value >>> 8;
                  this[offset + 1] = value & 0xff;
               } else {
                  objectWriteUInt16(this, value, offset, false);
               }
               return offset + 2;
            };

            Buffer.prototype.writeInt32LE = function writeInt32LE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 4, 0x7fffffff, -0x80000000);
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value & 0xff;
                  this[offset + 1] = value >>> 8;
                  this[offset + 2] = value >>> 16;
                  this[offset + 3] = value >>> 24;
               } else {
                  objectWriteUInt32(this, value, offset, true);
               }
               return offset + 4;
            };

            Buffer.prototype.writeInt32BE = function writeInt32BE(value, offset, noAssert) {
               value = +value;
               offset = offset | 0;
               if (!noAssert) checkInt(this, value, offset, 4, 0x7fffffff, -0x80000000);
               if (value < 0) value = 0xffffffff + value + 1;
               if (Buffer.TYPED_ARRAY_SUPPORT) {
                  this[offset] = value >>> 24;
                  this[offset + 1] = value >>> 16;
                  this[offset + 2] = value >>> 8;
                  this[offset + 3] = value & 0xff;
               } else {
                  objectWriteUInt32(this, value, offset, false);
               }
               return offset + 4;
            };

            function checkIEEE754(buf, value, offset, ext, max, min) {
               if (offset + ext > buf.length) throw new RangeError('Index out of range');
               if (offset < 0) throw new RangeError('Index out of range');
            }

            function writeFloat(buf, value, offset, littleEndian, noAssert) {
               if (!noAssert) {
                  checkIEEE754(buf, value, offset, 4, 3.4028234663852886e+38, -3.4028234663852886e+38);
               }
               ieee754.write(buf, value, offset, littleEndian, 23, 4);
               return offset + 4;
            }

            Buffer.prototype.writeFloatLE = function writeFloatLE(value, offset, noAssert) {
               return writeFloat(this, value, offset, true, noAssert);
            };

            Buffer.prototype.writeFloatBE = function writeFloatBE(value, offset, noAssert) {
               return writeFloat(this, value, offset, false, noAssert);
            };

            function writeDouble(buf, value, offset, littleEndian, noAssert) {
               if (!noAssert) {
                  checkIEEE754(buf, value, offset, 8, 1.7976931348623157E+308, -1.7976931348623157E+308);
               }
               ieee754.write(buf, value, offset, littleEndian, 52, 8);
               return offset + 8;
            }

            Buffer.prototype.writeDoubleLE = function writeDoubleLE(value, offset, noAssert) {
               return writeDouble(this, value, offset, true, noAssert);
            };

            Buffer.prototype.writeDoubleBE = function writeDoubleBE(value, offset, noAssert) {
               return writeDouble(this, value, offset, false, noAssert);
            };

            // copy(targetBuffer, targetStart=0, sourceStart=0, sourceEnd=buffer.length)
            Buffer.prototype.copy = function copy(target, targetStart, start, end) {
               if (!start) start = 0;
               if (!end && end !== 0) end = this.length;
               if (targetStart >= target.length) targetStart = target.length;
               if (!targetStart) targetStart = 0;
               if (end > 0 && end < start) end = start;

               // Copy 0 bytes; we're done
               if (end === start) return 0;
               if (target.length === 0 || this.length === 0) return 0;

               // Fatal error conditions
               if (targetStart < 0) {
                  throw new RangeError('targetStart out of bounds');
               }
               if (start < 0 || start >= this.length) throw new RangeError('sourceStart out of bounds');
               if (end < 0) throw new RangeError('sourceEnd out of bounds');

               // Are we oob?
               if (end > this.length) end = this.length;
               if (target.length - targetStart < end - start) {
                  end = target.length - targetStart + start;
               }

               var len = end - start;
               var i;

               if (this === target && start < targetStart && targetStart < end) {
                  // descending copy from end
                  for (i = len - 1; i >= 0; --i) {
                     target[i + targetStart] = this[i + start];
                  }
               } else if (len < 1000 || !Buffer.TYPED_ARRAY_SUPPORT) {
                  // ascending copy from start
                  for (i = 0; i < len; ++i) {
                     target[i + targetStart] = this[i + start];
                  }
               } else {
                  Uint8Array.prototype.set.call(target, this.subarray(start, start + len), targetStart);
               }

               return len;
            };

            // Usage:
            //    buffer.fill(number[, offset[, end]])
            //    buffer.fill(buffer[, offset[, end]])
            //    buffer.fill(string[, offset[, end]][, encoding])
            Buffer.prototype.fill = function fill(val, start, end, encoding) {
               // Handle string cases:
               if (typeof val === 'string') {
                  if (typeof start === 'string') {
                     encoding = start;
                     start = 0;
                     end = this.length;
                  } else if (typeof end === 'string') {
                     encoding = end;
                     end = this.length;
                  }
                  if (val.length === 1) {
                     var code = val.charCodeAt(0);
                     if (code < 256) {
                        val = code;
                     }
                  }
                  if (encoding !== undefined && typeof encoding !== 'string') {
                     throw new TypeError('encoding must be a string');
                  }
                  if (typeof encoding === 'string' && !Buffer.isEncoding(encoding)) {
                     throw new TypeError('Unknown encoding: ' + encoding);
                  }
               } else if (typeof val === 'number') {
                  val = val & 255;
               }

               // Invalid ranges are not set to a default, so can range check early.
               if (start < 0 || this.length < start || this.length < end) {
                  throw new RangeError('Out of range index');
               }

               if (end <= start) {
                  return this;
               }

               start = start >>> 0;
               end = end === undefined ? this.length : end >>> 0;

               if (!val) val = 0;

               var i;
               if (typeof val === 'number') {
                  for (i = start; i < end; ++i) {
                     this[i] = val;
                  }
               } else {
                  var bytes = Buffer.isBuffer(val) ? val : utf8ToBytes(new Buffer(val, encoding).toString());
                  var len = bytes.length;
                  for (i = 0; i < end - start; ++i) {
                     this[i + start] = bytes[i % len];
                  }
               }

               return this;
            };

            // HELPER FUNCTIONS
            // ================

            var INVALID_BASE64_RE = /[^+\/0-9A-Za-z-_]/g;

            function base64clean(str) {
               // Node strips out invalid characters like \n and \t from the string, base64-js does not
               str = stringtrim(str).replace(INVALID_BASE64_RE, '');
               // Node converts strings with length < 2 to ''
               if (str.length < 2) return '';
               // Node allows for non-padded base64 strings (missing trailing ===), base64-js does not
               while (str.length % 4 !== 0) {
                  str = str + '=';
               }
               return str;
            }

            function stringtrim(str) {
               if (str.trim) return str.trim();
               return str.replace(/^\s+|\s+$/g, '');
            }

            function toHex(n) {
               if (n < 16) return '0' + n.toString(16);
               return n.toString(16);
            }

            function utf8ToBytes(string, units) {
               units = units || Infinity;
               var codePoint;
               var length = string.length;
               var leadSurrogate = null;
               var bytes = [];

               for (var i = 0; i < length; ++i) {
                  codePoint = string.charCodeAt(i);

                  // is surrogate component
                  if (codePoint > 0xD7FF && codePoint < 0xE000) {
                     // last char was a lead
                     if (!leadSurrogate) {
                        // no lead yet
                        if (codePoint > 0xDBFF) {
                           // unexpected trail
                           if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD);
                           continue;
                        } else if (i + 1 === length) {
                           // unpaired lead
                           if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD);
                           continue;
                        }

                        // valid lead
                        leadSurrogate = codePoint;

                        continue;
                     }

                     // 2 leads in a row
                     if (codePoint < 0xDC00) {
                        if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD);
                        leadSurrogate = codePoint;
                        continue;
                     }

                     // valid surrogate pair
                     codePoint = (leadSurrogate - 0xD800 << 10 | codePoint - 0xDC00) + 0x10000;
                  } else if (leadSurrogate) {
                     // valid bmp char, but last char was a lead
                     if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD);
                  }

                  leadSurrogate = null;

                  // encode utf8
                  if (codePoint < 0x80) {
                     if ((units -= 1) < 0) break;
                     bytes.push(codePoint);
                  } else if (codePoint < 0x800) {
                     if ((units -= 2) < 0) break;
                     bytes.push(codePoint >> 0x6 | 0xC0, codePoint & 0x3F | 0x80);
                  } else if (codePoint < 0x10000) {
                     if ((units -= 3) < 0) break;
                     bytes.push(codePoint >> 0xC | 0xE0, codePoint >> 0x6 & 0x3F | 0x80, codePoint & 0x3F | 0x80);
                  } else if (codePoint < 0x110000) {
                     if ((units -= 4) < 0) break;
                     bytes.push(codePoint >> 0x12 | 0xF0, codePoint >> 0xC & 0x3F | 0x80, codePoint >> 0x6 & 0x3F | 0x80, codePoint & 0x3F | 0x80);
                  } else {
                     throw new Error('Invalid code point');
                  }
               }

               return bytes;
            }

            function asciiToBytes(str) {
               var byteArray = [];
               for (var i = 0; i < str.length; ++i) {
                  // Node's code seems to be doing this and not & 0x7F..
                  byteArray.push(str.charCodeAt(i) & 0xFF);
               }
               return byteArray;
            }

            function utf16leToBytes(str, units) {
               var c, hi, lo;
               var byteArray = [];
               for (var i = 0; i < str.length; ++i) {
                  if ((units -= 2) < 0) break;

                  c = str.charCodeAt(i);
                  hi = c >> 8;
                  lo = c % 256;
                  byteArray.push(lo);
                  byteArray.push(hi);
               }

               return byteArray;
            }

            function base64ToBytes(str) {
               return base64.toByteArray(base64clean(str));
            }

            function blitBuffer(src, dst, offset, length) {
               for (var i = 0; i < length; ++i) {
                  if (i + offset >= dst.length || i >= src.length) break;
                  dst[i + offset] = src[i];
               }
               return i;
            }

            function isnan(val) {
               return val !== val; // eslint-disable-line no-self-compare
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1)))

         /***/
      }),
/* 5 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (Buffer) {

            var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

            // Copyright Joyent, Inc. and other Node contributors.
            //
            // Permission is hereby granted, free of charge, to any person obtaining a
            // copy of this software and associated documentation files (the
            // "Software"), to deal in the Software without restriction, including
            // without limitation the rights to use, copy, modify, merge, publish,
            // distribute, sublicense, and/or sell copies of the Software, and to permit
            // persons to whom the Software is furnished to do so, subject to the
            // following conditions:
            //
            // The above copyright notice and this permission notice shall be included
            // in all copies or substantial portions of the Software.
            //
            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
            // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
            // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
            // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
            // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
            // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
            // USE OR OTHER DEALINGS IN THE SOFTWARE.

            // NOTE: These type checking functions intentionally don't use `instanceof`
            // because it is fragile and can be easily faked with `Object.create()`.

            function isArray(arg) {
               if (Array.isArray) {
                  return Array.isArray(arg);
               }
               return objectToString(arg) === '[object Array]';
            }
            exports.isArray = isArray;

            function isBoolean(arg) {
               return typeof arg === 'boolean';
            }
            exports.isBoolean = isBoolean;

            function isNull(arg) {
               return arg === null;
            }
            exports.isNull = isNull;

            function isNullOrUndefined(arg) {
               return arg == null;
            }
            exports.isNullOrUndefined = isNullOrUndefined;

            function isNumber(arg) {
               return typeof arg === 'number';
            }
            exports.isNumber = isNumber;

            function isString(arg) {
               return typeof arg === 'string';
            }
            exports.isString = isString;

            function isSymbol(arg) {
               return (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'symbol';
            }
            exports.isSymbol = isSymbol;

            function isUndefined(arg) {
               return arg === void 0;
            }
            exports.isUndefined = isUndefined;

            function isRegExp(re) {
               return objectToString(re) === '[object RegExp]';
            }
            exports.isRegExp = isRegExp;

            function isObject(arg) {
               return (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'object' && arg !== null;
            }
            exports.isObject = isObject;

            function isDate(d) {
               return objectToString(d) === '[object Date]';
            }
            exports.isDate = isDate;

            function isError(e) {
               return objectToString(e) === '[object Error]' || e instanceof Error;
            }
            exports.isError = isError;

            function isFunction(arg) {
               return typeof arg === 'function';
            }
            exports.isFunction = isFunction;

            function isPrimitive(arg) {
               return arg === null || typeof arg === 'boolean' || typeof arg === 'number' || typeof arg === 'string' || (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'symbol' || // ES6 symbol
                  typeof arg === 'undefined';
            }
            exports.isPrimitive = isPrimitive;

            exports.isBuffer = Buffer.isBuffer;

            function objectToString(o) {
               return Object.prototype.toString.call(o);
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(4).Buffer))

         /***/
      }),
/* 6 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         // shim for using process in browser
         var process = module.exports = {};

         // cached from whatever global is present so that test runners that stub it
         // don't break things.  But we need to wrap it in a try catch in case it is
         // wrapped in strict mode code which doesn't define any globals.  It's inside a
         // function because try/catches deoptimize in certain engines.

         var cachedSetTimeout;
         var cachedClearTimeout;

         function defaultSetTimout() {
            throw new Error('setTimeout has not been defined');
         }
         function defaultClearTimeout() {
            throw new Error('clearTimeout has not been defined');
         }
         (function () {
            try {
               if (typeof setTimeout === 'function') {
                  cachedSetTimeout = setTimeout;
               } else {
                  cachedSetTimeout = defaultSetTimout;
               }
            } catch (e) {
               cachedSetTimeout = defaultSetTimout;
            }
            try {
               if (typeof clearTimeout === 'function') {
                  cachedClearTimeout = clearTimeout;
               } else {
                  cachedClearTimeout = defaultClearTimeout;
               }
            } catch (e) {
               cachedClearTimeout = defaultClearTimeout;
            }
         })();
         function runTimeout(fun) {
            if (cachedSetTimeout === setTimeout) {
               //normal enviroments in sane situations
               return setTimeout(fun, 0);
            }
            // if setTimeout wasn't available but was latter defined
            if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
               cachedSetTimeout = setTimeout;
               return setTimeout(fun, 0);
            }
            try {
               // when when somebody has screwed with setTimeout but no I.E. maddness
               return cachedSetTimeout(fun, 0);
            } catch (e) {
               try {
                  // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
                  return cachedSetTimeout.call(null, fun, 0);
               } catch (e) {
                  // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
                  return cachedSetTimeout.call(this, fun, 0);
               }
            }
         }
         function runClearTimeout(marker) {
            if (cachedClearTimeout === clearTimeout) {
               //normal enviroments in sane situations
               return clearTimeout(marker);
            }
            // if clearTimeout wasn't available but was latter defined
            if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
               cachedClearTimeout = clearTimeout;
               return clearTimeout(marker);
            }
            try {
               // when when somebody has screwed with setTimeout but no I.E. maddness
               return cachedClearTimeout(marker);
            } catch (e) {
               try {
                  // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
                  return cachedClearTimeout.call(null, marker);
               } catch (e) {
                  // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
                  // Some versions of I.E. have different rules for clearTimeout vs setTimeout
                  return cachedClearTimeout.call(this, marker);
               }
            }
         }
         var queue = [];
         var draining = false;
         var currentQueue;
         var queueIndex = -1;

         function cleanUpNextTick() {
            if (!draining || !currentQueue) {
               return;
            }
            draining = false;
            if (currentQueue.length) {
               queue = currentQueue.concat(queue);
            } else {
               queueIndex = -1;
            }
            if (queue.length) {
               drainQueue();
            }
         }

         function drainQueue() {
            if (draining) {
               return;
            }
            var timeout = runTimeout(cleanUpNextTick);
            draining = true;

            var len = queue.length;
            while (len) {
               currentQueue = queue;
               queue = [];
               while (++queueIndex < len) {
                  if (currentQueue) {
                     currentQueue[queueIndex].run();
                  }
               }
               queueIndex = -1;
               len = queue.length;
            }
            currentQueue = null;
            draining = false;
            runClearTimeout(timeout);
         }

         process.nextTick = function (fun) {
            var args = new Array(arguments.length - 1);
            if (arguments.length > 1) {
               for (var i = 1; i < arguments.length; i++) {
                  args[i - 1] = arguments[i];
               }
            }
            queue.push(new Item(fun, args));
            if (queue.length === 1 && !draining) {
               runTimeout(drainQueue);
            }
         };

         // v8 likes predictible objects
         function Item(fun, array) {
            this.fun = fun;
            this.array = array;
         }
         Item.prototype.run = function () {
            this.fun.apply(null, this.array);
         };
         process.title = 'browser';
         process.browser = true;
         process.env = {};
         process.argv = [];
         process.version = ''; // empty string to avoid regexp issues
         process.versions = {};

         function noop() { }

         process.on = noop;
         process.addListener = noop;
         process.once = noop;
         process.off = noop;
         process.removeListener = noop;
         process.removeAllListeners = noop;
         process.emit = noop;
         process.prependListener = noop;
         process.prependOnceListener = noop;

         process.listeners = function (name) {
            return [];
         };

         process.binding = function (name) {
            throw new Error('process.binding is not supported');
         };

         process.cwd = function () {
            return '/';
         };
         process.chdir = function (dir) {
            throw new Error('process.chdir is not supported');
         };
         process.umask = function () {
            return 0;
         };

         /***/
      }),
/* 7 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         /* eslint-disable node/no-deprecated-api */
         var buffer = __webpack_require__(4);
         var Buffer = buffer.Buffer;

         // alternative to using Object.keys for old browsers
         function copyProps(src, dst) {
            for (var key in src) {
               dst[key] = src[key];
            }
         }
         if (Buffer.from && Buffer.alloc && Buffer.allocUnsafe && Buffer.allocUnsafeSlow) {
            module.exports = buffer;
         } else {
            // Copy properties from require('buffer')
            copyProps(buffer, exports);
            exports.Buffer = SafeBuffer;
         }

         function SafeBuffer(arg, encodingOrOffset, length) {
            return Buffer(arg, encodingOrOffset, length);
         }

         // Copy static methods from Buffer
         copyProps(Buffer, SafeBuffer);

         SafeBuffer.from = function (arg, encodingOrOffset, length) {
            if (typeof arg === 'number') {
               throw new TypeError('Argument must not be a number');
            }
            return Buffer(arg, encodingOrOffset, length);
         };

         SafeBuffer.alloc = function (size, fill, encoding) {
            if (typeof size !== 'number') {
               throw new TypeError('Argument must be a number');
            }
            var buf = Buffer(size);
            if (fill !== undefined) {
               if (typeof encoding === 'string') {
                  buf.fill(fill, encoding);
               } else {
                  buf.fill(fill);
               }
            } else {
               buf.fill(0);
            }
            return buf;
         };

         SafeBuffer.allocUnsafe = function (size) {
            if (typeof size !== 'number') {
               throw new TypeError('Argument must be a number');
            }
            return Buffer(size);
         };

         SafeBuffer.allocUnsafeSlow = function (size) {
            if (typeof size !== 'number') {
               throw new TypeError('Argument must be a number');
            }
            return buffer.SlowBuffer(size);
         };

         /***/
      }),
/* 8 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         Object.defineProperty(exports, "__esModule", { value: true });
         var ILogger_1 = __webpack_require__(3);

         var NullLogger = function () {
            function NullLogger() {
               _classCallCheck(this, NullLogger);
            }

            _createClass(NullLogger, [{
               key: "log",
               value: function log(logLevel, message) { }
            }]);

            return NullLogger;
         }();

         exports.NullLogger = NullLogger;

         var ConsoleLogger = function () {
            function ConsoleLogger(minimumLogLevel) {
               _classCallCheck(this, ConsoleLogger);

               this.minimumLogLevel = minimumLogLevel;
            }

            _createClass(ConsoleLogger, [{
               key: "log",
               value: function log(logLevel, message) {
                  if (logLevel >= this.minimumLogLevel) {
                     console.log(ILogger_1.LogLevel[logLevel] + ": " + message);
                  }
               }
            }]);

            return ConsoleLogger;
         }();

         exports.ConsoleLogger = ConsoleLogger;
         var LoggerFactory;
         (function (LoggerFactory) {
            function createLogger(logging) {
               if (logging === undefined) {
                  return new ConsoleLogger(ILogger_1.LogLevel.Information);
               }
               if (logging === null) {
                  return new NullLogger();
               }
               if (logging.log) {
                  return logging;
               }
               return new ConsoleLogger(logging);
            }
            LoggerFactory.createLogger = createLogger;
         })(LoggerFactory = exports.LoggerFactory || (exports.LoggerFactory = {}));

         /***/
      }),
/* 9 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (Buffer) {

            var DuplexStream = __webpack_require__(40),
               util = __webpack_require__(14);

            function BufferList(callback) {
               if (!(this instanceof BufferList)) return new BufferList(callback);

               this._bufs = [];
               this.length = 0;

               if (typeof callback == 'function') {
                  this._callback = callback;

                  var piper = function piper(err) {
                     if (this._callback) {
                        this._callback(err);
                        this._callback = null;
                     }
                  }.bind(this);

                  this.on('pipe', function onPipe(src) {
                     src.on('error', piper);
                  });
                  this.on('unpipe', function onUnpipe(src) {
                     src.removeListener('error', piper);
                  });
               } else {
                  this.append(callback);
               }

               DuplexStream.call(this);
            }

            util.inherits(BufferList, DuplexStream);

            BufferList.prototype._offset = function _offset(offset) {
               var tot = 0,
                  i = 0,
                  _t;
               if (offset === 0) return [0, 0];
               for (; i < this._bufs.length; i++) {
                  _t = tot + this._bufs[i].length;
                  if (offset < _t || i == this._bufs.length - 1) return [i, offset - tot];
                  tot = _t;
               }
            };

            BufferList.prototype.append = function append(buf) {
               var i = 0;

               if (Buffer.isBuffer(buf)) {
                  this._appendBuffer(buf);
               } else if (Array.isArray(buf)) {
                  for (; i < buf.length; i++) {
                     this.append(buf[i]);
                  }
               } else if (buf instanceof BufferList) {
                  // unwrap argument into individual BufferLists
                  for (; i < buf._bufs.length; i++) {
                     this.append(buf._bufs[i]);
                  }
               } else if (buf != null) {
                  // coerce number arguments to strings, since Buffer(number) does
                  // uninitialized memory allocation
                  if (typeof buf == 'number') buf = buf.toString();

                  this._appendBuffer(new Buffer(buf));
               }

               return this;
            };

            BufferList.prototype._appendBuffer = function appendBuffer(buf) {
               this._bufs.push(buf);
               this.length += buf.length;
            };

            BufferList.prototype._write = function _write(buf, encoding, callback) {
               this._appendBuffer(buf);

               if (typeof callback == 'function') callback();
            };

            BufferList.prototype._read = function _read(size) {
               if (!this.length) return this.push(null);

               size = Math.min(size, this.length);
               this.push(this.slice(0, size));
               this.consume(size);
            };

            BufferList.prototype.end = function end(chunk) {
               DuplexStream.prototype.end.call(this, chunk);

               if (this._callback) {
                  this._callback(null, this.slice());
                  this._callback = null;
               }
            };

            BufferList.prototype.get = function get(index) {
               return this.slice(index, index + 1)[0];
            };

            BufferList.prototype.slice = function slice(start, end) {
               if (typeof start == 'number' && start < 0) start += this.length;
               if (typeof end == 'number' && end < 0) end += this.length;
               return this.copy(null, 0, start, end);
            };

            BufferList.prototype.copy = function copy(dst, dstStart, srcStart, srcEnd) {
               if (typeof srcStart != 'number' || srcStart < 0) srcStart = 0;
               if (typeof srcEnd != 'number' || srcEnd > this.length) srcEnd = this.length;
               if (srcStart >= this.length) return dst || new Buffer(0);
               if (srcEnd <= 0) return dst || new Buffer(0);

               var copy = !!dst,
                  off = this._offset(srcStart),
                  len = srcEnd - srcStart,
                  bytes = len,
                  bufoff = copy && dstStart || 0,
                  start = off[1],
                  l,
                  i;

               // copy/slice everything
               if (srcStart === 0 && srcEnd == this.length) {
                  if (!copy) {
                     // slice, but full concat if multiple buffers
                     return this._bufs.length === 1 ? this._bufs[0] : Buffer.concat(this._bufs, this.length);
                  }

                  // copy, need to copy individual buffers
                  for (i = 0; i < this._bufs.length; i++) {
                     this._bufs[i].copy(dst, bufoff);
                     bufoff += this._bufs[i].length;
                  }

                  return dst;
               }

               // easy, cheap case where it's a subset of one of the buffers
               if (bytes <= this._bufs[off[0]].length - start) {
                  return copy ? this._bufs[off[0]].copy(dst, dstStart, start, start + bytes) : this._bufs[off[0]].slice(start, start + bytes);
               }

               if (!copy) // a slice, we need something to copy in to
                  dst = new Buffer(len);

               for (i = off[0]; i < this._bufs.length; i++) {
                  l = this._bufs[i].length - start;

                  if (bytes > l) {
                     this._bufs[i].copy(dst, bufoff, start);
                  } else {
                     this._bufs[i].copy(dst, bufoff, start, start + bytes);
                     break;
                  }

                  bufoff += l;
                  bytes -= l;

                  if (start) start = 0;
               }

               return dst;
            };

            BufferList.prototype.shallowSlice = function shallowSlice(start, end) {
               start = start || 0;
               end = end || this.length;

               if (start < 0) start += this.length;
               if (end < 0) end += this.length;

               var startOffset = this._offset(start),
                  endOffset = this._offset(end),
                  buffers = this._bufs.slice(startOffset[0], endOffset[0] + 1);

               if (endOffset[1] == 0) buffers.pop(); else buffers[buffers.length - 1] = buffers[buffers.length - 1].slice(0, endOffset[1]);

               if (startOffset[1] != 0) buffers[0] = buffers[0].slice(startOffset[1]);

               return new BufferList(buffers);
            };

            BufferList.prototype.toString = function toString(encoding, start, end) {
               return this.slice(start, end).toString(encoding);
            };

            BufferList.prototype.consume = function consume(bytes) {
               while (this._bufs.length) {
                  if (bytes >= this._bufs[0].length) {
                     bytes -= this._bufs[0].length;
                     this.length -= this._bufs[0].length;
                     this._bufs.shift();
                  } else {
                     this._bufs[0] = this._bufs[0].slice(bytes);
                     this.length -= bytes;
                     break;
                  }
               }
               return this;
            };

            BufferList.prototype.duplicate = function duplicate() {
               var i = 0,
                  copy = new BufferList();

               for (; i < this._bufs.length; i++) {
                  copy.append(this._bufs[i]);
               } return copy;
            };

            BufferList.prototype.destroy = function destroy() {
               this._bufs.length = 0;
               this.length = 0;
               this.push(null);
            }; (function () {
               var methods = {
                  'readDoubleBE': 8,
                  'readDoubleLE': 8,
                  'readFloatBE': 4,
                  'readFloatLE': 4,
                  'readInt32BE': 4,
                  'readInt32LE': 4,
                  'readUInt32BE': 4,
                  'readUInt32LE': 4,
                  'readInt16BE': 2,
                  'readInt16LE': 2,
                  'readUInt16BE': 2,
                  'readUInt16LE': 2,
                  'readInt8': 1,
                  'readUInt8': 1
               };

               for (var m in methods) {
                  (function (m) {
                     BufferList.prototype[m] = function (offset) {
                        return this.slice(offset, offset + methods[m])[m](0);
                     };
                  })(m);
               }
            })();

            module.exports = BufferList;
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(4).Buffer))

         /***/
      }),
/* 10 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (process) {

            if (!process.version || process.version.indexOf('v0.') === 0 || process.version.indexOf('v1.') === 0 && process.version.indexOf('v1.8.') !== 0) {
               module.exports = nextTick;
            } else {
               module.exports = process.nextTick;
            }

            function nextTick(fn, arg1, arg2, arg3) {
               if (typeof fn !== 'function') {
                  throw new TypeError('"callback" argument must be a function');
               }
               var len = arguments.length;
               var args, i;
               switch (len) {
                  case 0:
                  case 1:
                     return process.nextTick(fn);
                  case 2:
                     return process.nextTick(function afterTickOne() {
                        fn.call(null, arg1);
                     });
                  case 3:
                     return process.nextTick(function afterTickTwo() {
                        fn.call(null, arg1, arg2);
                     });
                  case 4:
                     return process.nextTick(function afterTickThree() {
                        fn.call(null, arg1, arg2, arg3);
                     });
                  default:
                     args = new Array(len - 1);
                     i = 0;
                     while (i < args.length) {
                        args[i++] = arguments[i];
                     }
                     return process.nextTick(function afterTick() {
                        fn.apply(null, args);
                     });
               }
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(6)))

         /***/
      }),
/* 11 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         Object.defineProperty(exports, "__esModule", { value: true });
         var TextMessageFormat;
         (function (TextMessageFormat) {
            var RecordSeparator = String.fromCharCode(0x1e);
            function write(output) {
               return "" + output + RecordSeparator;
            }
            TextMessageFormat.write = write;
            function parse(input) {
               if (input[input.length - 1] != RecordSeparator) {
                  throw new Error("Message is incomplete.");
               }
               var messages = input.split(RecordSeparator);
               messages.pop();
               return messages;
            }
            TextMessageFormat.parse = parse;
         })(TextMessageFormat = exports.TextMessageFormat || (exports.TextMessageFormat = {}));
         var BinaryMessageFormat;
         (function (BinaryMessageFormat) {
            // The length prefix of binary messages is encoded as VarInt. Read the comment in
            // the BinaryMessageParser.TryParseMessage for details.
            function write(output) {
               // msgpack5 uses returns Buffer instead of Uint8Array on IE10 and some other browser
               //  in which case .byteLength does will be undefined
               var size = output.byteLength || output.length;
               var lenBuffer = [];
               do {
                  var sizePart = size & 0x7f;
                  size = size >> 7;
                  if (size > 0) {
                     sizePart |= 0x80;
                  }
                  lenBuffer.push(sizePart);
               } while (size > 0);
               // msgpack5 uses returns Buffer instead of Uint8Array on IE10 and some other browser
               //  in which case .byteLength does will be undefined
               size = output.byteLength || output.length;
               var buffer = new Uint8Array(lenBuffer.length + size);
               buffer.set(lenBuffer, 0);
               buffer.set(output, lenBuffer.length);
               return buffer.buffer;
            }
            BinaryMessageFormat.write = write;
            function parse(input) {
               var result = [];
               var uint8Array = new Uint8Array(input);
               var maxLengthPrefixSize = 5;
               var numBitsToShift = [0, 7, 14, 21, 28];
               for (var offset = 0; offset < input.byteLength;) {
                  var numBytes = 0;
                  var size = 0;
                  var byteRead = void 0;
                  do {
                     byteRead = uint8Array[offset + numBytes];
                     size = size | (byteRead & 0x7f) << numBitsToShift[numBytes];
                     numBytes++;
                  } while (numBytes < Math.min(maxLengthPrefixSize, input.byteLength - offset) && (byteRead & 0x80) != 0);
                  if ((byteRead & 0x80) !== 0 && numBytes < maxLengthPrefixSize) {
                     throw new Error("Cannot read message size.");
                  }
                  if (numBytes === maxLengthPrefixSize && byteRead > 7) {
                     throw new Error("Messages bigger than 2GB are not supported.");
                  }
                  if (uint8Array.byteLength >= offset + numBytes + size) {
                     // IE does not support .slice() so use subarray
                     result.push(uint8Array.slice ? uint8Array.slice(offset + numBytes, offset + numBytes + size) : uint8Array.subarray(offset + numBytes, offset + numBytes + size));
                  } else {
                     throw new Error("Incomplete message.");
                  }
                  offset = offset + numBytes + size;
               }
               return result;
            }
            BinaryMessageFormat.parse = parse;
         })(BinaryMessageFormat = exports.BinaryMessageFormat || (exports.BinaryMessageFormat = {}));

         /***/
      }),
/* 12 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         var __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {
            return new (P || (P = Promise))(function (resolve, reject) {
               function fulfilled(value) {
                  try {
                     step(generator.next(value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function rejected(value) {
                  try {
                     step(generator["throw"](value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function step(result) {
                  result.done ? resolve(result.value) : new P(function (resolve) {
                     resolve(result.value);
                  }).then(fulfilled, rejected);
               }
               step((generator = generator.apply(thisArg, _arguments || [])).next());
            });
         };
         Object.defineProperty(exports, "__esModule", { value: true });
         var Transports_1 = __webpack_require__(13);
         var HttpClient_1 = __webpack_require__(16);
         var ILogger_1 = __webpack_require__(3);
         var Loggers_1 = __webpack_require__(8);

         var HttpConnection = function () {
            function HttpConnection(url) {
               var options = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};

               _classCallCheck(this, HttpConnection);

               this.features = {};
               this.logger = Loggers_1.LoggerFactory.createLogger(options.logging);
               this.url = this.resolveUrl(url);
               options = options || {};
               this.httpClient = options.httpClient || new HttpClient_1.HttpClient();
               this.connectionState = 0 /* Initial */;
               this.options = options;
            }

            _createClass(HttpConnection, [{
               key: "start",
               value: function start() {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee() {
                     return regeneratorRuntime.wrap(function _callee$(_context) {
                        while (1) {
                           switch (_context.prev = _context.next) {
                              case 0:
                                 if (!(this.connectionState != 0 /* Initial */)) {
                                    _context.next = 2;
                                    break;
                                 }

                                 return _context.abrupt("return", Promise.reject(new Error("Cannot start a connection that is not in the 'Initial' state.")));

                              case 2:
                                 this.connectionState = 1 /* Connecting */;
                                 this.startPromise = this.startInternal();
                                 return _context.abrupt("return", this.startPromise);

                              case 5:
                              case "end":
                                 return _context.stop();
                           }
                        }
                     }, _callee, this);
                  }));
               }
            }, {
               key: "startInternal",
               value: function startInternal() {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee2() {
                     var _this = this;

                     var negotiatePayload, negotiateResponse, requestedTransferMode;
                     return regeneratorRuntime.wrap(function _callee2$(_context2) {
                        while (1) {
                           switch (_context2.prev = _context2.next) {
                              case 0:
                                 _context2.prev = 0;
                                 _context2.next = 3;
                                 return this.httpClient.options(this.url);

                              case 3:
                                 negotiatePayload = _context2.sent;
                                 negotiateResponse = JSON.parse(negotiatePayload);

                                 this.connectionId = negotiateResponse.connectionId;
                                 // the user tries to stop the the connection when it is being started

                                 if (!(this.connectionState == 3 /* Disconnected */)) {
                                    _context2.next = 8;
                                    break;
                                 }

                                 return _context2.abrupt("return");

                              case 8:
                                 this.url += (this.url.indexOf("?") == -1 ? "?" : "&") + ("id=" + this.connectionId);
                                 this.transport = this.createTransport(this.options.transport, negotiateResponse.availableTransports);
                                 this.transport.onreceive = this.onreceive;
                                 this.transport.onclose = function (e) {
                                    return _this.stopConnection(true, e);
                                 };
                                 requestedTransferMode = this.features.transferMode === 2 /* Binary */
                                    ? 2 /* Binary */
                                    : 1;
                                 _context2.next = 15;
                                 return this.transport.connect(this.url, requestedTransferMode);

                              case 15:
                                 this.features.transferMode = _context2.sent;

                                 // only change the state if we were connecting to not overwrite
                                 // the state if the connection is already marked as Disconnected
                                 this.changeState(1 /* Connecting */, 2 /* Connected */);
                                 _context2.next = 25;
                                 break;

                              case 19:
                                 _context2.prev = 19;
                                 _context2.t0 = _context2["catch"](0);

                                 this.logger.log(ILogger_1.LogLevel.Error, "Failed to start the connection. " + _context2.t0);
                                 this.connectionState = 3 /* Disconnected */;
                                 this.transport = null;
                                 throw _context2.t0;

                              case 25:
                                 ;

                              case 26:
                              case "end":
                                 return _context2.stop();
                           }
                        }
                     }, _callee2, this, [[0, 19]]);
                  }));
               }
            }, {
               key: "createTransport",
               value: function createTransport(transport, availableTransports) {
                  if (!transport && availableTransports.length > 0) {
                     transport = Transports_1.TransportType[availableTransports[0]];
                  }
                  if (transport === Transports_1.TransportType.WebSockets && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
                     return new Transports_1.WebSocketTransport(this.logger);
                  }
                  if (transport === Transports_1.TransportType.ServerSentEvents && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
                     return new Transports_1.ServerSentEventsTransport(this.httpClient, this.logger);
                  }
                  if (transport === Transports_1.TransportType.LongPolling && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
                     return new Transports_1.LongPollingTransport(this.httpClient, this.logger);
                  }
                  if (this.isITransport(transport)) {
                     return transport;
                  }
                  throw new Error("No available transports found.");
               }
            }, {
               key: "isITransport",
               value: function isITransport(transport) {
                  return (typeof transport === "undefined" ? "undefined" : _typeof(transport)) === "object" && "connect" in transport;
               }
            }, {
               key: "changeState",
               value: function changeState(from, to) {
                  if (this.connectionState == from) {
                     this.connectionState = to;
                     return true;
                  }
                  return false;
               }
            }, {
               key: "send",
               value: function send(data) {
                  if (this.connectionState != 2 /* Connected */) {
                     throw new Error("Cannot send data if the connection is not in the 'Connected' State");
                  }
                  return this.transport.send(data);
               }
            }, {
               key: "stop",
               value: function stop() {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee3() {
                     var previousState;
                     return regeneratorRuntime.wrap(function _callee3$(_context3) {
                        while (1) {
                           switch (_context3.prev = _context3.next) {
                              case 0:
                                 previousState = this.connectionState;

                                 this.connectionState = 3 /* Disconnected */;
                                 _context3.prev = 2;
                                 _context3.next = 5;
                                 return this.startPromise;

                              case 5:
                                 _context3.next = 9;
                                 break;

                              case 7:
                                 _context3.prev = 7;
                                 _context3.t0 = _context3["catch"](2);

                              case 9:
                                 this.stopConnection( /*raiseClosed*/previousState == 2 /* Connected */);

                              case 10:
                              case "end":
                                 return _context3.stop();
                           }
                        }
                     }, _callee3, this, [[2, 7]]);
                  }));
               }
            }, {
               key: "stopConnection",
               value: function stopConnection(raiseClosed, error) {
                  if (this.transport) {
                     this.transport.stop();
                     this.transport = null;
                  }
                  this.connectionState = 3 /* Disconnected */;
                  if (raiseClosed && this.onclose) {
                     this.onclose(error);
                  }
               }
            }, {
               key: "resolveUrl",
               value: function resolveUrl(url) {
                  // startsWith is not supported in IE
                  if (url.lastIndexOf("https://", 0) === 0 || url.lastIndexOf("http://", 0) === 0) {
                     return url;
                  }
                  if (typeof window === 'undefined' || !window || !window.document) {
                     throw new Error("Cannot resolve '" + url + "'.");
                  }
                  var parser = window.document.createElement("a");
                  parser.href = url;
                  var baseUrl = !parser.protocol || parser.protocol === ":" ? window.document.location.protocol + "//" + (parser.host || window.document.location.host) : parser.protocol + "//" + parser.host;
                  if (!url || url[0] != '/') {
                     url = '/' + url;
                  }
                  var normalizedUrl = baseUrl + url;
                  this.logger.log(ILogger_1.LogLevel.Information, "Normalizing '" + url + "' to '" + normalizedUrl + "'");
                  return normalizedUrl;
               }
            }]);

            return HttpConnection;
         }();

         exports.HttpConnection = HttpConnection;

         /***/
      }),
/* 13 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         var __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {
            return new (P || (P = Promise))(function (resolve, reject) {
               function fulfilled(value) {
                  try {
                     step(generator.next(value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function rejected(value) {
                  try {
                     step(generator["throw"](value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function step(result) {
                  result.done ? resolve(result.value) : new P(function (resolve) {
                     resolve(result.value);
                  }).then(fulfilled, rejected);
               }
               step((generator = generator.apply(thisArg, _arguments || [])).next());
            });
         };
         Object.defineProperty(exports, "__esModule", { value: true });
         var HttpError_1 = __webpack_require__(17);
         var ILogger_1 = __webpack_require__(3);
         var TransportType;
         (function (TransportType) {
            TransportType[TransportType["WebSockets"] = 0] = "WebSockets";
            TransportType[TransportType["ServerSentEvents"] = 1] = "ServerSentEvents";
            TransportType[TransportType["LongPolling"] = 2] = "LongPolling";
         })(TransportType = exports.TransportType || (exports.TransportType = {}));

         var WebSocketTransport = function () {
            function WebSocketTransport(logger) {
               _classCallCheck(this, WebSocketTransport);

               this.logger = logger;
            }

            _createClass(WebSocketTransport, [{
               key: "connect",
               value: function connect(url, requestedTransferMode) {
                  var _this = this;

                  return new Promise(function (resolve, reject) {
                     url = url.replace(/^http/, "ws");
                     var webSocket = new WebSocket(url);
                     if (requestedTransferMode == 2 /* Binary */) {
                        webSocket.binaryType = "arraybuffer";
                     }
                     webSocket.onopen = function (event) {
                        _this.logger.log(ILogger_1.LogLevel.Information, "WebSocket connected to " + url);
                        _this.webSocket = webSocket;
                        resolve(requestedTransferMode);
                     };
                     webSocket.onerror = function (event) {
                        reject();
                     };
                     webSocket.onmessage = function (message) {
                        _this.logger.log(ILogger_1.LogLevel.Trace, "(WebSockets transport) data received: " + message.data);
                        if (_this.onreceive) {
                           _this.onreceive(message.data);
                        }
                     };
                     webSocket.onclose = function (event) {
                        // webSocket will be null if the transport did not start successfully
                        if (_this.onclose && _this.webSocket) {
                           if (event.wasClean === false || event.code !== 1000) {
                              _this.onclose(new Error("Websocket closed with status code: " + event.code + " (" + event.reason + ")"));
                           } else {
                              _this.onclose();
                           }
                        }
                     };
                  });
               }
            }, {
               key: "send",
               value: function send(data) {
                  if (this.webSocket && this.webSocket.readyState === WebSocket.OPEN) {
                     this.webSocket.send(data);
                     return Promise.resolve();
                  }
                  return Promise.reject("WebSocket is not in the OPEN state");
               }
            }, {
               key: "stop",
               value: function stop() {
                  if (this.webSocket) {
                     this.webSocket.close();
                     this.webSocket = null;
                  }
               }
            }]);

            return WebSocketTransport;
         }();

         exports.WebSocketTransport = WebSocketTransport;

         var ServerSentEventsTransport = function () {
            function ServerSentEventsTransport(httpClient, logger) {
               _classCallCheck(this, ServerSentEventsTransport);

               this.httpClient = httpClient;
               this.logger = logger;
            }

            _createClass(ServerSentEventsTransport, [{
               key: "connect",
               value: function connect(url, requestedTransferMode) {
                  var _this2 = this;

                  if (typeof EventSource === "undefined") {
                     Promise.reject("EventSource not supported by the browser.");
                  }
                  this.url = url;
                  return new Promise(function (resolve, reject) {
                     var eventSource = new EventSource(_this2.url);
                     try {
                        eventSource.onmessage = function (e) {
                           if (_this2.onreceive) {
                              try {
                                 _this2.logger.log(ILogger_1.LogLevel.Trace, "(SSE transport) data received: " + e.data);
                                 _this2.onreceive(e.data);
                              } catch (error) {
                                 if (_this2.onclose) {
                                    _this2.onclose(error);
                                 }
                                 return;
                              }
                           }
                        };
                        eventSource.onerror = function (e) {
                           reject();
                           // don't report an error if the transport did not start successfully
                           if (_this2.eventSource && _this2.onclose) {
                              _this2.onclose(new Error(e.message || "Error occurred"));
                           }
                        };
                        eventSource.onopen = function () {
                           _this2.logger.log(ILogger_1.LogLevel.Information, "SSE connected to " + _this2.url);
                           _this2.eventSource = eventSource;
                           // SSE is a text protocol
                           resolve(1 /* Text */);
                        };
                     } catch (e) {
                        return Promise.reject(e);
                     }
                  });
               }
            }, {
               key: "send",
               value: function send(data) {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee() {
                     return regeneratorRuntime.wrap(function _callee$(_context) {
                        while (1) {
                           switch (_context.prev = _context.next) {
                              case 0:
                                 return _context.abrupt("return", _send(this.httpClient, this.url, data));

                              case 1:
                              case "end":
                                 return _context.stop();
                           }
                        }
                     }, _callee, this);
                  }));
               }
            }, {
               key: "stop",
               value: function stop() {
                  if (this.eventSource) {
                     this.eventSource.close();
                     this.eventSource = null;
                  }
               }
            }]);

            return ServerSentEventsTransport;
         }();

         exports.ServerSentEventsTransport = ServerSentEventsTransport;

         var LongPollingTransport = function () {
            function LongPollingTransport(httpClient, logger) {
               _classCallCheck(this, LongPollingTransport);

               this.httpClient = httpClient;
               this.logger = logger;
            }

            _createClass(LongPollingTransport, [{
               key: "connect",
               value: function connect(url, requestedTransferMode) {
                  this.url = url;
                  this.shouldPoll = true;
                  if (requestedTransferMode === 2 /* Binary */ && typeof new XMLHttpRequest().responseType !== "string") {
                     // This will work if we fix: https://github.com/aspnet/SignalR/issues/742
                     throw new Error("Binary protocols over XmlHttpRequest not implementing advanced features are not supported.");
                  }
                  this.poll(this.url, requestedTransferMode);
                  return Promise.resolve(requestedTransferMode);
               }
            }, {
               key: "poll",
               value: function poll(url, transferMode) {
                  var _this3 = this;

                  if (!this.shouldPoll) {
                     return;
                  }
                  var pollXhr = new XMLHttpRequest();
                  pollXhr.onload = function () {
                     if (pollXhr.status == 200) {
                        if (_this3.onreceive) {
                           try {
                              var response = transferMode === 1 /* Text */
                                 ? pollXhr.responseText : pollXhr.response;
                              if (response) {
                                 _this3.logger.log(ILogger_1.LogLevel.Trace, "(LongPolling transport) data received: " + response);
                                 _this3.onreceive(response);
                              } else {
                                 _this3.logger.log(ILogger_1.LogLevel.Information, "(LongPolling transport) timed out");
                              }
                           } catch (error) {
                              if (_this3.onclose) {
                                 _this3.onclose(error);
                              }
                              return;
                           }
                        }
                        _this3.poll(url, transferMode);
                     } else if (_this3.pollXhr.status == 204) {
                        if (_this3.onclose) {
                           _this3.onclose();
                        }
                     } else {
                        if (_this3.onclose) {
                           _this3.onclose(new HttpError_1.HttpError(pollXhr.statusText, pollXhr.status));
                        }
                     }
                  };
                  pollXhr.onerror = function () {
                     if (_this3.onclose) {
                        // network related error or denied cross domain request
                        _this3.onclose(new Error("Sending HTTP request failed."));
                     }
                  };
                  pollXhr.ontimeout = function () {
                     _this3.poll(url, transferMode);
                  };
                  this.pollXhr = pollXhr;
                  this.pollXhr.open("GET", url + "&_=" + Date.now(), true);
                  if (transferMode === 2 /* Binary */) {
                     this.pollXhr.responseType = "arraybuffer";
                  }
                  // TODO: consider making timeout configurable
                  this.pollXhr.timeout = 120000;
                  this.pollXhr.send();
               }
            }, {
               key: "send",
               value: function send(data) {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee2() {
                     return regeneratorRuntime.wrap(function _callee2$(_context2) {
                        while (1) {
                           switch (_context2.prev = _context2.next) {
                              case 0:
                                 return _context2.abrupt("return", _send(this.httpClient, this.url, data));

                              case 1:
                              case "end":
                                 return _context2.stop();
                           }
                        }
                     }, _callee2, this);
                  }));
               }
            }, {
               key: "stop",
               value: function stop() {
                  this.shouldPoll = false;
                  if (this.pollXhr) {
                     this.pollXhr.abort();
                     this.pollXhr = null;
                  }
               }
            }]);

            return LongPollingTransport;
         }();

         exports.LongPollingTransport = LongPollingTransport;
         var headers = new Map();
         function _send(httpClient, url, data) {
            return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee3() {
               return regeneratorRuntime.wrap(function _callee3$(_context3) {
                  while (1) {
                     switch (_context3.prev = _context3.next) {
                        case 0:
                           _context3.next = 2;
                           return httpClient.post(url, data, headers);

                        case 2:
                        case "end":
                           return _context3.stop();
                     }
                  }
               }, _callee3, this);
            }));
         }

         /***/
      }),
/* 14 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global, process) {

            var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

            // Copyright Joyent, Inc. and other Node contributors.
            //
            // Permission is hereby granted, free of charge, to any person obtaining a
            // copy of this software and associated documentation files (the
            // "Software"), to deal in the Software without restriction, including
            // without limitation the rights to use, copy, modify, merge, publish,
            // distribute, sublicense, and/or sell copies of the Software, and to permit
            // persons to whom the Software is furnished to do so, subject to the
            // following conditions:
            //
            // The above copyright notice and this permission notice shall be included
            // in all copies or substantial portions of the Software.
            //
            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
            // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
            // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
            // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
            // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
            // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
            // USE OR OTHER DEALINGS IN THE SOFTWARE.

            var formatRegExp = /%[sdj%]/g;
            exports.format = function (f) {
               if (!isString(f)) {
                  var objects = [];
                  for (var i = 0; i < arguments.length; i++) {
                     objects.push(inspect(arguments[i]));
                  }
                  return objects.join(' ');
               }

               var i = 1;
               var args = arguments;
               var len = args.length;
               var str = String(f).replace(formatRegExp, function (x) {
                  if (x === '%%') return '%';
                  if (i >= len) return x;
                  switch (x) {
                     case '%s':
                        return String(args[i++]);
                     case '%d':
                        return Number(args[i++]);
                     case '%j':
                        try {
                           return JSON.stringify(args[i++]);
                        } catch (_) {
                           return '[Circular]';
                        }
                     default:
                        return x;
                  }
               });
               for (var x = args[i]; i < len; x = args[++i]) {
                  if (isNull(x) || !isObject(x)) {
                     str += ' ' + x;
                  } else {
                     str += ' ' + inspect(x);
                  }
               }
               return str;
            };

            // Mark that a method should not be used.
            // Returns a modified function which warns once by default.
            // If --no-deprecation is set, then it is a no-op.
            exports.deprecate = function (fn, msg) {
               // Allow for deprecating things in the process of starting up.
               if (isUndefined(global.process)) {
                  return function () {
                     return exports.deprecate(fn, msg).apply(this, arguments);
                  };
               }

               if (process.noDeprecation === true) {
                  return fn;
               }

               var warned = false;
               function deprecated() {
                  if (!warned) {
                     if (process.throwDeprecation) {
                        throw new Error(msg);
                     } else if (process.traceDeprecation) {
                        console.trace(msg);
                     } else {
                        console.error(msg);
                     }
                     warned = true;
                  }
                  return fn.apply(this, arguments);
               }

               return deprecated;
            };

            var debugs = {};
            var debugEnviron;
            exports.debuglog = function (set) {
               if (isUndefined(debugEnviron)) debugEnviron = process.env.NODE_DEBUG || '';
               set = set.toUpperCase();
               if (!debugs[set]) {
                  if (new RegExp('\\b' + set + '\\b', 'i').test(debugEnviron)) {
                     var pid = process.pid;
                     debugs[set] = function () {
                        var msg = exports.format.apply(exports, arguments);
                        console.error('%s %d: %s', set, pid, msg);
                     };
                  } else {
                     debugs[set] = function () { };
                  }
               }
               return debugs[set];
            };

            /**
             * Echos the value of a value. Trys to print the value out
             * in the best way possible given the different types.
             *
             * @param {Object} obj The object to print out.
             * @param {Object} opts Optional options object that alters the output.
             */
            /* legacy: obj, showHidden, depth, colors*/
            function inspect(obj, opts) {
               // default options
               var ctx = {
                  seen: [],
                  stylize: stylizeNoColor
               };
               // legacy...
               if (arguments.length >= 3) ctx.depth = arguments[2];
               if (arguments.length >= 4) ctx.colors = arguments[3];
               if (isBoolean(opts)) {
                  // legacy...
                  ctx.showHidden = opts;
               } else if (opts) {
                  // got an "options" object
                  exports._extend(ctx, opts);
               }
               // set default options
               if (isUndefined(ctx.showHidden)) ctx.showHidden = false;
               if (isUndefined(ctx.depth)) ctx.depth = 2;
               if (isUndefined(ctx.colors)) ctx.colors = false;
               if (isUndefined(ctx.customInspect)) ctx.customInspect = true;
               if (ctx.colors) ctx.stylize = stylizeWithColor;
               return formatValue(ctx, obj, ctx.depth);
            }
            exports.inspect = inspect;

            // http://en.wikipedia.org/wiki/ANSI_escape_code#graphics
            inspect.colors = {
               'bold': [1, 22],
               'italic': [3, 23],
               'underline': [4, 24],
               'inverse': [7, 27],
               'white': [37, 39],
               'grey': [90, 39],
               'black': [30, 39],
               'blue': [34, 39],
               'cyan': [36, 39],
               'green': [32, 39],
               'magenta': [35, 39],
               'red': [31, 39],
               'yellow': [33, 39]
            };

            // Don't use 'blue' not visible on cmd.exe
            inspect.styles = {
               'special': 'cyan',
               'number': 'yellow',
               'boolean': 'yellow',
               'undefined': 'grey',
               'null': 'bold',
               'string': 'green',
               'date': 'magenta',
               // "name": intentionally not styling
               'regexp': 'red'
            };

            function stylizeWithColor(str, styleType) {
               var style = inspect.styles[styleType];

               if (style) {
                  return '\x1B[' + inspect.colors[style][0] + 'm' + str + '\x1B[' + inspect.colors[style][1] + 'm';
               } else {
                  return str;
               }
            }

            function stylizeNoColor(str, styleType) {
               return str;
            }

            function arrayToHash(array) {
               var hash = {};

               array.forEach(function (val, idx) {
                  hash[val] = true;
               });

               return hash;
            }

            function formatValue(ctx, value, recurseTimes) {
               // Provide a hook for user-specified inspect functions.
               // Check that value is an object with an inspect function on it
               if (ctx.customInspect && value && isFunction(value.inspect) &&
                  // Filter out the util module, it's inspect function is special
                  value.inspect !== exports.inspect &&
                  // Also filter out any prototype objects using the circular check.
                  !(value.constructor && value.constructor.prototype === value)) {
                  var ret = value.inspect(recurseTimes, ctx);
                  if (!isString(ret)) {
                     ret = formatValue(ctx, ret, recurseTimes);
                  }
                  return ret;
               }

               // Primitive types cannot have properties
               var primitive = formatPrimitive(ctx, value);
               if (primitive) {
                  return primitive;
               }

               // Look up the keys of the object.
               var keys = Object.keys(value);
               var visibleKeys = arrayToHash(keys);

               if (ctx.showHidden) {
                  keys = Object.getOwnPropertyNames(value);
               }

               // IE doesn't make error fields non-enumerable
               // http://msdn.microsoft.com/en-us/library/ie/dww52sbt(v=vs.94).aspx
               if (isError(value) && (keys.indexOf('message') >= 0 || keys.indexOf('description') >= 0)) {
                  return formatError(value);
               }

               // Some type of object without properties can be shortcutted.
               if (keys.length === 0) {
                  if (isFunction(value)) {
                     var name = value.name ? ': ' + value.name : '';
                     return ctx.stylize('[Function' + name + ']', 'special');
                  }
                  if (isRegExp(value)) {
                     return ctx.stylize(RegExp.prototype.toString.call(value), 'regexp');
                  }
                  if (isDate(value)) {
                     return ctx.stylize(Date.prototype.toString.call(value), 'date');
                  }
                  if (isError(value)) {
                     return formatError(value);
                  }
               }

               var base = '',
                  array = false,
                  braces = ['{', '}'];

               // Make Array say that they are Array
               if (isArray(value)) {
                  array = true;
                  braces = ['[', ']'];
               }

               // Make functions say that they are functions
               if (isFunction(value)) {
                  var n = value.name ? ': ' + value.name : '';
                  base = ' [Function' + n + ']';
               }

               // Make RegExps say that they are RegExps
               if (isRegExp(value)) {
                  base = ' ' + RegExp.prototype.toString.call(value);
               }

               // Make dates with properties first say the date
               if (isDate(value)) {
                  base = ' ' + Date.prototype.toUTCString.call(value);
               }

               // Make error with message first say the error
               if (isError(value)) {
                  base = ' ' + formatError(value);
               }

               if (keys.length === 0 && (!array || value.length == 0)) {
                  return braces[0] + base + braces[1];
               }

               if (recurseTimes < 0) {
                  if (isRegExp(value)) {
                     return ctx.stylize(RegExp.prototype.toString.call(value), 'regexp');
                  } else {
                     return ctx.stylize('[Object]', 'special');
                  }
               }

               ctx.seen.push(value);

               var output;
               if (array) {
                  output = formatArray(ctx, value, recurseTimes, visibleKeys, keys);
               } else {
                  output = keys.map(function (key) {
                     return formatProperty(ctx, value, recurseTimes, visibleKeys, key, array);
                  });
               }

               ctx.seen.pop();

               return reduceToSingleString(output, base, braces);
            }

            function formatPrimitive(ctx, value) {
               if (isUndefined(value)) return ctx.stylize('undefined', 'undefined');
               if (isString(value)) {
                  var simple = '\'' + JSON.stringify(value).replace(/^"|"$/g, '').replace(/'/g, "\\'").replace(/\\"/g, '"') + '\'';
                  return ctx.stylize(simple, 'string');
               }
               if (isNumber(value)) return ctx.stylize('' + value, 'number');
               if (isBoolean(value)) return ctx.stylize('' + value, 'boolean');
               // For some reason typeof null is "object", so special case here.
               if (isNull(value)) return ctx.stylize('null', 'null');
            }

            function formatError(value) {
               return '[' + Error.prototype.toString.call(value) + ']';
            }

            function formatArray(ctx, value, recurseTimes, visibleKeys, keys) {
               var output = [];
               for (var i = 0, l = value.length; i < l; ++i) {
                  if (hasOwnProperty(value, String(i))) {
                     output.push(formatProperty(ctx, value, recurseTimes, visibleKeys, String(i), true));
                  } else {
                     output.push('');
                  }
               }
               keys.forEach(function (key) {
                  if (!key.match(/^\d+$/)) {
                     output.push(formatProperty(ctx, value, recurseTimes, visibleKeys, key, true));
                  }
               });
               return output;
            }

            function formatProperty(ctx, value, recurseTimes, visibleKeys, key, array) {
               var name, str, desc;
               desc = Object.getOwnPropertyDescriptor(value, key) || { value: value[key] };
               if (desc.get) {
                  if (desc.set) {
                     str = ctx.stylize('[Getter/Setter]', 'special');
                  } else {
                     str = ctx.stylize('[Getter]', 'special');
                  }
               } else {
                  if (desc.set) {
                     str = ctx.stylize('[Setter]', 'special');
                  }
               }
               if (!hasOwnProperty(visibleKeys, key)) {
                  name = '[' + key + ']';
               }
               if (!str) {
                  if (ctx.seen.indexOf(desc.value) < 0) {
                     if (isNull(recurseTimes)) {
                        str = formatValue(ctx, desc.value, null);
                     } else {
                        str = formatValue(ctx, desc.value, recurseTimes - 1);
                     }
                     if (str.indexOf('\n') > -1) {
                        if (array) {
                           str = str.split('\n').map(function (line) {
                              return '  ' + line;
                           }).join('\n').substr(2);
                        } else {
                           str = '\n' + str.split('\n').map(function (line) {
                              return '   ' + line;
                           }).join('\n');
                        }
                     }
                  } else {
                     str = ctx.stylize('[Circular]', 'special');
                  }
               }
               if (isUndefined(name)) {
                  if (array && key.match(/^\d+$/)) {
                     return str;
                  }
                  name = JSON.stringify('' + key);
                  if (name.match(/^"([a-zA-Z_][a-zA-Z_0-9]*)"$/)) {
                     name = name.substr(1, name.length - 2);
                     name = ctx.stylize(name, 'name');
                  } else {
                     name = name.replace(/'/g, "\\'").replace(/\\"/g, '"').replace(/(^"|"$)/g, "'");
                     name = ctx.stylize(name, 'string');
                  }
               }

               return name + ': ' + str;
            }

            function reduceToSingleString(output, base, braces) {
               var numLinesEst = 0;
               var length = output.reduce(function (prev, cur) {
                  numLinesEst++;
                  if (cur.indexOf('\n') >= 0) numLinesEst++;
                  return prev + cur.replace(/\u001b\[\d\d?m/g, '').length + 1;
               }, 0);

               if (length > 60) {
                  return braces[0] + (base === '' ? '' : base + '\n ') + ' ' + output.join(',\n  ') + ' ' + braces[1];
               }

               return braces[0] + base + ' ' + output.join(', ') + ' ' + braces[1];
            }

            // NOTE: These type checking functions intentionally don't use `instanceof`
            // because it is fragile and can be easily faked with `Object.create()`.
            function isArray(ar) {
               return Array.isArray(ar);
            }
            exports.isArray = isArray;

            function isBoolean(arg) {
               return typeof arg === 'boolean';
            }
            exports.isBoolean = isBoolean;

            function isNull(arg) {
               return arg === null;
            }
            exports.isNull = isNull;

            function isNullOrUndefined(arg) {
               return arg == null;
            }
            exports.isNullOrUndefined = isNullOrUndefined;

            function isNumber(arg) {
               return typeof arg === 'number';
            }
            exports.isNumber = isNumber;

            function isString(arg) {
               return typeof arg === 'string';
            }
            exports.isString = isString;

            function isSymbol(arg) {
               return (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'symbol';
            }
            exports.isSymbol = isSymbol;

            function isUndefined(arg) {
               return arg === void 0;
            }
            exports.isUndefined = isUndefined;

            function isRegExp(re) {
               return isObject(re) && objectToString(re) === '[object RegExp]';
            }
            exports.isRegExp = isRegExp;

            function isObject(arg) {
               return (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'object' && arg !== null;
            }
            exports.isObject = isObject;

            function isDate(d) {
               return isObject(d) && objectToString(d) === '[object Date]';
            }
            exports.isDate = isDate;

            function isError(e) {
               return isObject(e) && (objectToString(e) === '[object Error]' || e instanceof Error);
            }
            exports.isError = isError;

            function isFunction(arg) {
               return typeof arg === 'function';
            }
            exports.isFunction = isFunction;

            function isPrimitive(arg) {
               return arg === null || typeof arg === 'boolean' || typeof arg === 'number' || typeof arg === 'string' || (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'symbol' || // ES6 symbol
                  typeof arg === 'undefined';
            }
            exports.isPrimitive = isPrimitive;

            exports.isBuffer = __webpack_require__(49);

            function objectToString(o) {
               return Object.prototype.toString.call(o);
            }

            function pad(n) {
               return n < 10 ? '0' + n.toString(10) : n.toString(10);
            }

            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

            // 26 Feb 16:19:34
            function timestamp() {
               var d = new Date();
               var time = [pad(d.getHours()), pad(d.getMinutes()), pad(d.getSeconds())].join(':');
               return [d.getDate(), months[d.getMonth()], time].join(' ');
            }

            // log is just a thin wrapper to console.log that prepends a timestamp
            exports.log = function () {
               console.log('%s - %s', timestamp(), exports.format.apply(exports, arguments));
            };

            /**
             * Inherit the prototype methods from one constructor into another.
             *
             * The Function.prototype.inherits from lang.js rewritten as a standalone
             * function (not on Function.prototype). NOTE: If this file is to be loaded
             * during bootstrapping this function needs to be rewritten using some native
             * functions as prototype setup using normal JavaScript does not work as
             * expected during bootstrapping (see mirror.js in r114903).
             *
             * @param {function} ctor Constructor function which needs to inherit the
             *     prototype.
             * @param {function} superCtor Constructor function to inherit prototype from.
             */
            exports.inherits = __webpack_require__(48);

            exports._extend = function (origin, add) {
               // Don't do anything if add isn't an object
               if (!add || !isObject(add)) return origin;

               var keys = Object.keys(add);
               var i = keys.length;
               while (i--) {
                  origin[keys[i]] = add[keys[i]];
               }
               return origin;
            };

            function hasOwnProperty(obj, prop) {
               return Object.prototype.hasOwnProperty.call(obj, prop);
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1), __webpack_require__(6)))

         /***/
      }),
/* 15 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         // This method of obtaining a reference to the global object needs to be
         // kept identical to the way it is obtained in runtime.js
         var g = function () {
            return this;
         }() || Function("return this")();

         // Use `getOwnPropertyNames` because not all browsers support calling
         // `hasOwnProperty` on the global `self` object in a worker. See #183.
         var hadRuntime = g.regeneratorRuntime && Object.getOwnPropertyNames(g).indexOf("regeneratorRuntime") >= 0;

         // Save the old regeneratorRuntime in case it needs to be restored later.
         var oldRuntime = hadRuntime && g.regeneratorRuntime;

         // Force reevalutation of runtime.js.
         g.regeneratorRuntime = undefined;

         module.exports = __webpack_require__(44);

         if (hadRuntime) {
            // Restore the original runtime.
            g.regeneratorRuntime = oldRuntime;
         } else {
            // Remove the global property added by runtime.js.
            try {
               delete g.regeneratorRuntime;
            } catch (e) {
               g.regeneratorRuntime = undefined;
            }
         }

         /***/
      }),
/* 16 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         Object.defineProperty(exports, "__esModule", { value: true });
         var HttpError_1 = __webpack_require__(17);

         var HttpClient = function () {
            function HttpClient() {
               _classCallCheck(this, HttpClient);
            }

            _createClass(HttpClient, [{
               key: "get",
               value: function get(url, headers) {
                  return this.xhr("GET", url, headers);
               }
            }, {
               key: "options",
               value: function options(url, headers) {
                  return this.xhr("OPTIONS", url, headers);
               }
            }, {
               key: "post",
               value: function post(url, content, headers) {
                  return this.xhr("POST", url, headers, content);
               }
            }, {
               key: "xhr",
               value: function xhr(method, url, headers, content) {
                  return new Promise(function (resolve, reject) {
                     var xhr = new XMLHttpRequest();
                     xhr.open(method, url, true);
                     xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                     if (headers) {
                        headers.forEach(function (value, header) {
                           return xhr.setRequestHeader(header, value);
                        });
                     }
                     xhr.send(content);
                     xhr.onload = function () {
                        if (xhr.status >= 200 && xhr.status < 300) {
                           resolve(xhr.response || xhr.responseText);
                        } else {
                           reject(new HttpError_1.HttpError(xhr.statusText, xhr.status));
                        }
                     };
                     xhr.onerror = function () {
                        reject(new HttpError_1.HttpError(xhr.statusText, xhr.status));
                     };
                  });
               }
            }]);

            return HttpClient;
         }();

         exports.HttpClient = HttpClient;

         /***/
      }),
/* 17 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

         function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

         Object.defineProperty(exports, "__esModule", { value: true });

         var HttpError = function (_Error) {
            _inherits(HttpError, _Error);

            function HttpError(errorMessage, statusCode) {
               _classCallCheck(this, HttpError);

               var _this = _possibleConstructorReturn(this, (HttpError.__proto__ || Object.getPrototypeOf(HttpError)).call(this, errorMessage));

               _this.statusCode = statusCode;
               return _this;
            }

            return HttpError;
         }(Error);

         exports.HttpError = HttpError;

         /***/
      }),
/* 18 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         Object.defineProperty(exports, "__esModule", { value: true });
         var Formatters_1 = __webpack_require__(11);

         var JsonHubProtocol = function () {
            function JsonHubProtocol() {
               _classCallCheck(this, JsonHubProtocol);

               this.name = "json";
               this.type = 1 /* Text */;
            }

            _createClass(JsonHubProtocol, [{
               key: "parseMessages",
               value: function parseMessages(input) {
                  if (!input) {
                     return [];
                  }
                  // Parse the messages
                  var messages = Formatters_1.TextMessageFormat.parse(input);
                  var hubMessages = [];
                  for (var i = 0; i < messages.length; ++i) {
                     hubMessages.push(JSON.parse(messages[i]));
                  }
                  return hubMessages;
               }
            }, {
               key: "writeMessage",
               value: function writeMessage(message) {
                  return Formatters_1.TextMessageFormat.write(JSON.stringify(message));
               }
            }]);

            return JsonHubProtocol;
         }();

         exports.JsonHubProtocol = JsonHubProtocol;

         /***/
      }),
/* 19 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

         // Copyright Joyent, Inc. and other Node contributors.
         //
         // Permission is hereby granted, free of charge, to any person obtaining a
         // copy of this software and associated documentation files (the
         // "Software"), to deal in the Software without restriction, including
         // without limitation the rights to use, copy, modify, merge, publish,
         // distribute, sublicense, and/or sell copies of the Software, and to permit
         // persons to whom the Software is furnished to do so, subject to the
         // following conditions:
         //
         // The above copyright notice and this permission notice shall be included
         // in all copies or substantial portions of the Software.
         //
         // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
         // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
         // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
         // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
         // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
         // USE OR OTHER DEALINGS IN THE SOFTWARE.

         function EventEmitter() {
            this._events = this._events || {};
            this._maxListeners = this._maxListeners || undefined;
         }
         module.exports = EventEmitter;

         // Backwards-compat with node 0.10.x
         EventEmitter.EventEmitter = EventEmitter;

         EventEmitter.prototype._events = undefined;
         EventEmitter.prototype._maxListeners = undefined;

         // By default EventEmitters will print a warning if more than 10 listeners are
         // added to it. This is a useful default which helps finding memory leaks.
         EventEmitter.defaultMaxListeners = 10;

         // Obviously not all Emitters should be limited to 10. This function allows
         // that to be increased. Set to zero for unlimited.
         EventEmitter.prototype.setMaxListeners = function (n) {
            if (!isNumber(n) || n < 0 || isNaN(n)) throw TypeError('n must be a positive number');
            this._maxListeners = n;
            return this;
         };

         EventEmitter.prototype.emit = function (type) {
            var er, handler, len, args, i, listeners;

            if (!this._events) this._events = {};

            // If there is no 'error' event listener then throw.
            if (type === 'error') {
               if (!this._events.error || isObject(this._events.error) && !this._events.error.length) {
                  er = arguments[1];
                  if (er instanceof Error) {
                     throw er; // Unhandled 'error' event
                  } else {
                     // At least give some kind of context to the user
                     var err = new Error('Uncaught, unspecified "error" event. (' + er + ')');
                     err.context = er;
                     throw err;
                  }
               }
            }

            handler = this._events[type];

            if (isUndefined(handler)) return false;

            if (isFunction(handler)) {
               switch (arguments.length) {
                  // fast cases
                  case 1:
                     handler.call(this);
                     break;
                  case 2:
                     handler.call(this, arguments[1]);
                     break;
                  case 3:
                     handler.call(this, arguments[1], arguments[2]);
                     break;
                  // slower
                  default:
                     args = Array.prototype.slice.call(arguments, 1);
                     handler.apply(this, args);
               }
            } else if (isObject(handler)) {
               args = Array.prototype.slice.call(arguments, 1);
               listeners = handler.slice();
               len = listeners.length;
               for (i = 0; i < len; i++) {
                  listeners[i].apply(this, args);
               }
            }

            return true;
         };

         EventEmitter.prototype.addListener = function (type, listener) {
            var m;

            if (!isFunction(listener)) throw TypeError('listener must be a function');

            if (!this._events) this._events = {};

            // To avoid recursion in the case that type === "newListener"! Before
            // adding it to the listeners, first emit "newListener".
            if (this._events.newListener) this.emit('newListener', type, isFunction(listener.listener) ? listener.listener : listener);

            if (!this._events[type])
               // Optimize the case of one listener. Don't need the extra array object.
               this._events[type] = listener; else if (isObject(this._events[type]))
               // If we've already got an array, just append.
               this._events[type].push(listener); else
               // Adding the second element, need to change to array.
               this._events[type] = [this._events[type], listener];

            // Check for listener leak
            if (isObject(this._events[type]) && !this._events[type].warned) {
               if (!isUndefined(this._maxListeners)) {
                  m = this._maxListeners;
               } else {
                  m = EventEmitter.defaultMaxListeners;
               }

               if (m && m > 0 && this._events[type].length > m) {
                  this._events[type].warned = true;
                  console.error('(node) warning: possible EventEmitter memory ' + 'leak detected. %d listeners added. ' + 'Use emitter.setMaxListeners() to increase limit.', this._events[type].length);
                  if (typeof console.trace === 'function') {
                     // not supported in IE 10
                     console.trace();
                  }
               }
            }

            return this;
         };

         EventEmitter.prototype.on = EventEmitter.prototype.addListener;

         EventEmitter.prototype.once = function (type, listener) {
            if (!isFunction(listener)) throw TypeError('listener must be a function');

            var fired = false;

            function g() {
               this.removeListener(type, g);

               if (!fired) {
                  fired = true;
                  listener.apply(this, arguments);
               }
            }

            g.listener = listener;
            this.on(type, g);

            return this;
         };

         // emits a 'removeListener' event iff the listener was removed
         EventEmitter.prototype.removeListener = function (type, listener) {
            var list, position, length, i;

            if (!isFunction(listener)) throw TypeError('listener must be a function');

            if (!this._events || !this._events[type]) return this;

            list = this._events[type];
            length = list.length;
            position = -1;

            if (list === listener || isFunction(list.listener) && list.listener === listener) {
               delete this._events[type];
               if (this._events.removeListener) this.emit('removeListener', type, listener);
            } else if (isObject(list)) {
               for (i = length; i-- > 0;) {
                  if (list[i] === listener || list[i].listener && list[i].listener === listener) {
                     position = i;
                     break;
                  }
               }

               if (position < 0) return this;

               if (list.length === 1) {
                  list.length = 0;
                  delete this._events[type];
               } else {
                  list.splice(position, 1);
               }

               if (this._events.removeListener) this.emit('removeListener', type, listener);
            }

            return this;
         };

         EventEmitter.prototype.removeAllListeners = function (type) {
            var key, listeners;

            if (!this._events) return this;

            // not listening for removeListener, no need to emit
            if (!this._events.removeListener) {
               if (arguments.length === 0) this._events = {}; else if (this._events[type]) delete this._events[type];
               return this;
            }

            // emit removeListener for all listeners on all events
            if (arguments.length === 0) {
               for (key in this._events) {
                  if (key === 'removeListener') continue;
                  this.removeAllListeners(key);
               }
               this.removeAllListeners('removeListener');
               this._events = {};
               return this;
            }

            listeners = this._events[type];

            if (isFunction(listeners)) {
               this.removeListener(type, listeners);
            } else if (listeners) {
               // LIFO order
               while (listeners.length) {
                  this.removeListener(type, listeners[listeners.length - 1]);
               }
            }
            delete this._events[type];

            return this;
         };

         EventEmitter.prototype.listeners = function (type) {
            var ret;
            if (!this._events || !this._events[type]) ret = []; else if (isFunction(this._events[type])) ret = [this._events[type]]; else ret = this._events[type].slice();
            return ret;
         };

         EventEmitter.prototype.listenerCount = function (type) {
            if (this._events) {
               var evlistener = this._events[type];

               if (isFunction(evlistener)) return 1; else if (evlistener) return evlistener.length;
            }
            return 0;
         };

         EventEmitter.listenerCount = function (emitter, type) {
            return emitter.listenerCount(type);
         };

         function isFunction(arg) {
            return typeof arg === 'function';
         }

         function isNumber(arg) {
            return typeof arg === 'number';
         }

         function isObject(arg) {
            return (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'object' && arg !== null;
         }

         function isUndefined(arg) {
            return arg === void 0;
         }

         /***/
      }),
/* 20 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var toString = {}.toString;

         module.exports = Array.isArray || function (arr) {
            return toString.call(arr) == '[object Array]';
         };

         /***/
      }),
/* 21 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         // Copyright Joyent, Inc. and other Node contributors.
         //
         // Permission is hereby granted, free of charge, to any person obtaining a
         // copy of this software and associated documentation files (the
         // "Software"), to deal in the Software without restriction, including
         // without limitation the rights to use, copy, modify, merge, publish,
         // distribute, sublicense, and/or sell copies of the Software, and to permit
         // persons to whom the Software is furnished to do so, subject to the
         // following conditions:
         //
         // The above copyright notice and this permission notice shall be included
         // in all copies or substantial portions of the Software.
         //
         // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
         // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
         // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
         // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
         // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
         // USE OR OTHER DEALINGS IN THE SOFTWARE.

         var Buffer = __webpack_require__(4).Buffer;

         var isBufferEncoding = Buffer.isEncoding || function (encoding) {
            switch (encoding && encoding.toLowerCase()) {
               case 'hex': case 'utf8': case 'utf-8': case 'ascii': case 'binary': case 'base64': case 'ucs2': case 'ucs-2': case 'utf16le': case 'utf-16le': case 'raw':
                  return true;
               default:
                  return false;
            }
         };

         function assertEncoding(encoding) {
            if (encoding && !isBufferEncoding(encoding)) {
               throw new Error('Unknown encoding: ' + encoding);
            }
         }

         // StringDecoder provides an interface for efficiently splitting a series of
         // buffers into a series of JS strings without breaking apart multi-byte
         // characters. CESU-8 is handled as part of the UTF-8 encoding.
         //
         // @TODO Handling all encodings inside a single object makes it very difficult
         // to reason about this code, so it should be split up in the future.
         // @TODO There should be a utf8-strict encoding that rejects invalid UTF-8 code
         // points as used by CESU-8.
         var StringDecoder = exports.StringDecoder = function (encoding) {
            this.encoding = (encoding || 'utf8').toLowerCase().replace(/[-_]/, '');
            assertEncoding(encoding);
            switch (this.encoding) {
               case 'utf8':
                  // CESU-8 represents each of Surrogate Pair by 3-bytes
                  this.surrogateSize = 3;
                  break;
               case 'ucs2':
               case 'utf16le':
                  // UTF-16 represents each of Surrogate Pair by 2-bytes
                  this.surrogateSize = 2;
                  this.detectIncompleteChar = utf16DetectIncompleteChar;
                  break;
               case 'base64':
                  // Base-64 stores 3 bytes in 4 chars, and pads the remainder.
                  this.surrogateSize = 3;
                  this.detectIncompleteChar = base64DetectIncompleteChar;
                  break;
               default:
                  this.write = passThroughWrite;
                  return;
            }

            // Enough space to store all bytes of a single character. UTF-8 needs 4
            // bytes, but CESU-8 may require up to 6 (3 bytes per surrogate).
            this.charBuffer = new Buffer(6);
            // Number of bytes received for the current incomplete multi-byte character.
            this.charReceived = 0;
            // Number of bytes expected for the current incomplete multi-byte character.
            this.charLength = 0;
         };

         // write decodes the given buffer and returns it as JS string that is
         // guaranteed to not contain any partial multi-byte characters. Any partial
         // character found at the end of the buffer is buffered up, and will be
         // returned when calling write again with the remaining bytes.
         //
         // Note: Converting a Buffer containing an orphan surrogate to a String
         // currently works, but converting a String to a Buffer (via `new Buffer`, or
         // Buffer#write) will replace incomplete surrogates with the unicode
         // replacement character. See https://codereview.chromium.org/121173009/ .
         StringDecoder.prototype.write = function (buffer) {
            var charStr = '';
            // if our last write ended with an incomplete multibyte character
            while (this.charLength) {
               // determine how many remaining bytes this buffer has to offer for this char
               var available = buffer.length >= this.charLength - this.charReceived ? this.charLength - this.charReceived : buffer.length;

               // add the new bytes to the char buffer
               buffer.copy(this.charBuffer, this.charReceived, 0, available);
               this.charReceived += available;

               if (this.charReceived < this.charLength) {
                  // still not enough chars in this buffer? wait for more ...
                  return '';
               }

               // remove bytes belonging to the current character from the buffer
               buffer = buffer.slice(available, buffer.length);

               // get the character that was split
               charStr = this.charBuffer.slice(0, this.charLength).toString(this.encoding);

               // CESU-8: lead surrogate (D800-DBFF) is also the incomplete character
               var charCode = charStr.charCodeAt(charStr.length - 1);
               if (charCode >= 0xD800 && charCode <= 0xDBFF) {
                  this.charLength += this.surrogateSize;
                  charStr = '';
                  continue;
               }
               this.charReceived = this.charLength = 0;

               // if there are no more bytes in this buffer, just emit our char
               if (buffer.length === 0) {
                  return charStr;
               }
               break;
            }

            // determine and set charLength / charReceived
            this.detectIncompleteChar(buffer);

            var end = buffer.length;
            if (this.charLength) {
               // buffer the incomplete character bytes we got
               buffer.copy(this.charBuffer, 0, buffer.length - this.charReceived, end);
               end -= this.charReceived;
            }

            charStr += buffer.toString(this.encoding, 0, end);

            var end = charStr.length - 1;
            var charCode = charStr.charCodeAt(end);
            // CESU-8: lead surrogate (D800-DBFF) is also the incomplete character
            if (charCode >= 0xD800 && charCode <= 0xDBFF) {
               var size = this.surrogateSize;
               this.charLength += size;
               this.charReceived += size;
               this.charBuffer.copy(this.charBuffer, size, 0, size);
               buffer.copy(this.charBuffer, 0, 0, size);
               return charStr.substring(0, end);
            }

            // or just emit the charStr
            return charStr;
         };

         // detectIncompleteChar determines if there is an incomplete UTF-8 character at
         // the end of the given buffer. If so, it sets this.charLength to the byte
         // length that character, and sets this.charReceived to the number of bytes
         // that are available for this character.
         StringDecoder.prototype.detectIncompleteChar = function (buffer) {
            // determine how many bytes we have to check at the end of this buffer
            var i = buffer.length >= 3 ? 3 : buffer.length;

            // Figure out if one of the last i bytes of our buffer announces an
            // incomplete char.
            for (; i > 0; i--) {
               var c = buffer[buffer.length - i];

               // See http://en.wikipedia.org/wiki/UTF-8#Description

               // 110XXXXX
               if (i == 1 && c >> 5 == 0x06) {
                  this.charLength = 2;
                  break;
               }

               // 1110XXXX
               if (i <= 2 && c >> 4 == 0x0E) {
                  this.charLength = 3;
                  break;
               }

               // 11110XXX
               if (i <= 3 && c >> 3 == 0x1E) {
                  this.charLength = 4;
                  break;
               }
            }
            this.charReceived = i;
         };

         StringDecoder.prototype.end = function (buffer) {
            var res = '';
            if (buffer && buffer.length) res = this.write(buffer);

            if (this.charReceived) {
               var cr = this.charReceived;
               var buf = this.charBuffer;
               var enc = this.encoding;
               res += buf.slice(0, cr).toString(enc);
            }

            return res;
         };

         function passThroughWrite(buffer) {
            return buffer.toString(this.encoding);
         }

         function utf16DetectIncompleteChar(buffer) {
            this.charReceived = buffer.length % 2;
            this.charLength = this.charReceived ? 2 : 0;
         }

         function base64DetectIncompleteChar(buffer) {
            this.charReceived = buffer.length % 3;
            this.charLength = this.charReceived ? 3 : 0;
         }

         /***/
      }),
/* 22 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global, process) {// Copyright Joyent, Inc. and other Node contributors.
            //
            // Permission is hereby granted, free of charge, to any person obtaining a
            // copy of this software and associated documentation files (the
            // "Software"), to deal in the Software without restriction, including
            // without limitation the rights to use, copy, modify, merge, publish,
            // distribute, sublicense, and/or sell copies of the Software, and to permit
            // persons to whom the Software is furnished to do so, subject to the
            // following conditions:
            //
            // The above copyright notice and this permission notice shall be included
            // in all copies or substantial portions of the Software.
            //
            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
            // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
            // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
            // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
            // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
            // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
            // USE OR OTHER DEALINGS IN THE SOFTWARE.



            /*<replacement>*/

            var processNextTick = __webpack_require__(10);
            /*</replacement>*/

            module.exports = Readable;

            /*<replacement>*/
            var isArray = __webpack_require__(20);
            /*</replacement>*/

            /*<replacement>*/
            var Duplex;
            /*</replacement>*/

            Readable.ReadableState = ReadableState;

            /*<replacement>*/
            var EE = __webpack_require__(19).EventEmitter;

            var EElistenerCount = function EElistenerCount(emitter, type) {
               return emitter.listeners(type).length;
            };
            /*</replacement>*/

            /*<replacement>*/
            var Stream = __webpack_require__(26);
            /*</replacement>*/

            // TODO(bmeurer): Change this back to const once hole checks are
            // properly optimized away early in Ignition+TurboFan.
            /*<replacement>*/
            var Buffer = __webpack_require__(7).Buffer;
            var OurUint8Array = global.Uint8Array || function () { };
            function _uint8ArrayToBuffer(chunk) {
               return Buffer.from(chunk);
            }
            function _isUint8Array(obj) {
               return Buffer.isBuffer(obj) || obj instanceof OurUint8Array;
            }
            /*</replacement>*/

            /*<replacement>*/
            var util = __webpack_require__(5);
            util.inherits = __webpack_require__(2);
            /*</replacement>*/

            /*<replacement>*/
            var debugUtil = __webpack_require__(51);
            var debug = void 0;
            if (debugUtil && debugUtil.debuglog) {
               debug = debugUtil.debuglog('stream');
            } else {
               debug = function debug() { };
            }
            /*</replacement>*/

            var BufferList = __webpack_require__(42);
            var destroyImpl = __webpack_require__(25);
            var StringDecoder;

            util.inherits(Readable, Stream);

            var kProxyEvents = ['error', 'close', 'destroy', 'pause', 'resume'];

            function prependListener(emitter, event, fn) {
               // Sadly this is not cacheable as some libraries bundle their own
               // event emitter implementation with them.
               if (typeof emitter.prependListener === 'function') {
                  return emitter.prependListener(event, fn);
               } else {
                  // This is a hack to make sure that our error handler is attached before any
                  // userland ones.  NEVER DO THIS. This is here only because this code needs
                  // to continue to work with older versions of Node.js that do not include
                  // the prependListener() method. The goal is to eventually remove this hack.
                  if (!emitter._events || !emitter._events[event]) emitter.on(event, fn); else if (isArray(emitter._events[event])) emitter._events[event].unshift(fn); else emitter._events[event] = [fn, emitter._events[event]];
               }
            }

            function ReadableState(options, stream) {
               Duplex = Duplex || __webpack_require__(0);

               options = options || {};

               // object stream flag. Used to make read(n) ignore n and to
               // make all the buffer merging and length checks go away
               this.objectMode = !!options.objectMode;

               if (stream instanceof Duplex) this.objectMode = this.objectMode || !!options.readableObjectMode;

               // the point at which it stops calling _read() to fill the buffer
               // Note: 0 is a valid value, means "don't call _read preemptively ever"
               var hwm = options.highWaterMark;
               var defaultHwm = this.objectMode ? 16 : 16 * 1024;
               this.highWaterMark = hwm || hwm === 0 ? hwm : defaultHwm;

               // cast to ints.
               this.highWaterMark = Math.floor(this.highWaterMark);

               // A linked list is used to store data chunks instead of an array because the
               // linked list can remove elements from the beginning faster than
               // array.shift()
               this.buffer = new BufferList();
               this.length = 0;
               this.pipes = null;
               this.pipesCount = 0;
               this.flowing = null;
               this.ended = false;
               this.endEmitted = false;
               this.reading = false;

               // a flag to be able to tell if the event 'readable'/'data' is emitted
               // immediately, or on a later tick.  We set this to true at first, because
               // any actions that shouldn't happen until "later" should generally also
               // not happen before the first read call.
               this.sync = true;

               // whenever we return null, then we set a flag to say
               // that we're awaiting a 'readable' event emission.
               this.needReadable = false;
               this.emittedReadable = false;
               this.readableListening = false;
               this.resumeScheduled = false;

               // has it been destroyed
               this.destroyed = false;

               // Crypto is kind of old and crusty.  Historically, its default string
               // encoding is 'binary' so we have to make this configurable.
               // Everything else in the universe uses 'utf8', though.
               this.defaultEncoding = options.defaultEncoding || 'utf8';

               // the number of writers that are awaiting a drain event in .pipe()s
               this.awaitDrain = 0;

               // if true, a maybeReadMore has been scheduled
               this.readingMore = false;

               this.decoder = null;
               this.encoding = null;
               if (options.encoding) {
                  if (!StringDecoder) StringDecoder = __webpack_require__(21).StringDecoder;
                  this.decoder = new StringDecoder(options.encoding);
                  this.encoding = options.encoding;
               }
            }

            function Readable(options) {
               Duplex = Duplex || __webpack_require__(0);

               if (!(this instanceof Readable)) return new Readable(options);

               this._readableState = new ReadableState(options, this);

               // legacy
               this.readable = true;

               if (options) {
                  if (typeof options.read === 'function') this._read = options.read;

                  if (typeof options.destroy === 'function') this._destroy = options.destroy;
               }

               Stream.call(this);
            }

            Object.defineProperty(Readable.prototype, 'destroyed', {
               get: function get() {
                  if (this._readableState === undefined) {
                     return false;
                  }
                  return this._readableState.destroyed;
               },
               set: function set(value) {
                  // we ignore the value if the stream
                  // has not been initialized yet
                  if (!this._readableState) {
                     return;
                  }

                  // backward compatibility, the user is explicitly
                  // managing destroyed
                  this._readableState.destroyed = value;
               }
            });

            Readable.prototype.destroy = destroyImpl.destroy;
            Readable.prototype._undestroy = destroyImpl.undestroy;
            Readable.prototype._destroy = function (err, cb) {
               this.push(null);
               cb(err);
            };

            // Manually shove something into the read() buffer.
            // This returns true if the highWaterMark has not been hit yet,
            // similar to how Writable.write() returns true if you should
            // write() some more.
            Readable.prototype.push = function (chunk, encoding) {
               var state = this._readableState;
               var skipChunkCheck;

               if (!state.objectMode) {
                  if (typeof chunk === 'string') {
                     encoding = encoding || state.defaultEncoding;
                     if (encoding !== state.encoding) {
                        chunk = Buffer.from(chunk, encoding);
                        encoding = '';
                     }
                     skipChunkCheck = true;
                  }
               } else {
                  skipChunkCheck = true;
               }

               return readableAddChunk(this, chunk, encoding, false, skipChunkCheck);
            };

            // Unshift should *always* be something directly out of read()
            Readable.prototype.unshift = function (chunk) {
               return readableAddChunk(this, chunk, null, true, false);
            };

            function readableAddChunk(stream, chunk, encoding, addToFront, skipChunkCheck) {
               var state = stream._readableState;
               if (chunk === null) {
                  state.reading = false;
                  onEofChunk(stream, state);
               } else {
                  var er;
                  if (!skipChunkCheck) er = chunkInvalid(state, chunk);
                  if (er) {
                     stream.emit('error', er);
                  } else if (state.objectMode || chunk && chunk.length > 0) {
                     if (typeof chunk !== 'string' && !state.objectMode && Object.getPrototypeOf(chunk) !== Buffer.prototype) {
                        chunk = _uint8ArrayToBuffer(chunk);
                     }

                     if (addToFront) {
                        if (state.endEmitted) stream.emit('error', new Error('stream.unshift() after end event')); else addChunk(stream, state, chunk, true);
                     } else if (state.ended) {
                        stream.emit('error', new Error('stream.push() after EOF'));
                     } else {
                        state.reading = false;
                        if (state.decoder && !encoding) {
                           chunk = state.decoder.write(chunk);
                           if (state.objectMode || chunk.length !== 0) addChunk(stream, state, chunk, false); else maybeReadMore(stream, state);
                        } else {
                           addChunk(stream, state, chunk, false);
                        }
                     }
                  } else if (!addToFront) {
                     state.reading = false;
                  }
               }

               return needMoreData(state);
            }

            function addChunk(stream, state, chunk, addToFront) {
               if (state.flowing && state.length === 0 && !state.sync) {
                  stream.emit('data', chunk);
                  stream.read(0);
               } else {
                  // update the buffer info.
                  state.length += state.objectMode ? 1 : chunk.length;
                  if (addToFront) state.buffer.unshift(chunk); else state.buffer.push(chunk);

                  if (state.needReadable) emitReadable(stream);
               }
               maybeReadMore(stream, state);
            }

            function chunkInvalid(state, chunk) {
               var er;
               if (!_isUint8Array(chunk) && typeof chunk !== 'string' && chunk !== undefined && !state.objectMode) {
                  er = new TypeError('Invalid non-string/buffer chunk');
               }
               return er;
            }

            // if it's past the high water mark, we can push in some more.
            // Also, if we have no data yet, we can stand some
            // more bytes.  This is to work around cases where hwm=0,
            // such as the repl.  Also, if the push() triggered a
            // readable event, and the user called read(largeNumber) such that
            // needReadable was set, then we ought to push more, so that another
            // 'readable' event will be triggered.
            function needMoreData(state) {
               return !state.ended && (state.needReadable || state.length < state.highWaterMark || state.length === 0);
            }

            Readable.prototype.isPaused = function () {
               return this._readableState.flowing === false;
            };

            // backwards compatibility.
            Readable.prototype.setEncoding = function (enc) {
               if (!StringDecoder) StringDecoder = __webpack_require__(21).StringDecoder;
               this._readableState.decoder = new StringDecoder(enc);
               this._readableState.encoding = enc;
               return this;
            };

            // Don't raise the hwm > 8MB
            var MAX_HWM = 0x800000;
            function computeNewHighWaterMark(n) {
               if (n >= MAX_HWM) {
                  n = MAX_HWM;
               } else {
                  // Get the next highest power of 2 to prevent increasing hwm excessively in
                  // tiny amounts
                  n--;
                  n |= n >>> 1;
                  n |= n >>> 2;
                  n |= n >>> 4;
                  n |= n >>> 8;
                  n |= n >>> 16;
                  n++;
               }
               return n;
            }

            // This function is designed to be inlinable, so please take care when making
            // changes to the function body.
            function howMuchToRead(n, state) {
               if (n <= 0 || state.length === 0 && state.ended) return 0;
               if (state.objectMode) return 1;
               if (n !== n) {
                  // Only flow one buffer at a time
                  if (state.flowing && state.length) return state.buffer.head.data.length; else return state.length;
               }
               // If we're asking for more than the current hwm, then raise the hwm.
               if (n > state.highWaterMark) state.highWaterMark = computeNewHighWaterMark(n);
               if (n <= state.length) return n;
               // Don't have enough
               if (!state.ended) {
                  state.needReadable = true;
                  return 0;
               }
               return state.length;
            }

            // you can override either this method, or the async _read(n) below.
            Readable.prototype.read = function (n) {
               debug('read', n);
               n = parseInt(n, 10);
               var state = this._readableState;
               var nOrig = n;

               if (n !== 0) state.emittedReadable = false;

               // if we're doing read(0) to trigger a readable event, but we
               // already have a bunch of data in the buffer, then just trigger
               // the 'readable' event and move on.
               if (n === 0 && state.needReadable && (state.length >= state.highWaterMark || state.ended)) {
                  debug('read: emitReadable', state.length, state.ended);
                  if (state.length === 0 && state.ended) endReadable(this); else emitReadable(this);
                  return null;
               }

               n = howMuchToRead(n, state);

               // if we've ended, and we're now clear, then finish it up.
               if (n === 0 && state.ended) {
                  if (state.length === 0) endReadable(this);
                  return null;
               }

               // All the actual chunk generation logic needs to be
               // *below* the call to _read.  The reason is that in certain
               // synthetic stream cases, such as passthrough streams, _read
               // may be a completely synchronous operation which may change
               // the state of the read buffer, providing enough data when
               // before there was *not* enough.
               //
               // So, the steps are:
               // 1. Figure out what the state of things will be after we do
               // a read from the buffer.
               //
               // 2. If that resulting state will trigger a _read, then call _read.
               // Note that this may be asynchronous, or synchronous.  Yes, it is
               // deeply ugly to write APIs this way, but that still doesn't mean
               // that the Readable class should behave improperly, as streams are
               // designed to be sync/async agnostic.
               // Take note if the _read call is sync or async (ie, if the read call
               // has returned yet), so that we know whether or not it's safe to emit
               // 'readable' etc.
               //
               // 3. Actually pull the requested chunks out of the buffer and return.

               // if we need a readable event, then we need to do some reading.
               var doRead = state.needReadable;
               debug('need readable', doRead);

               // if we currently have less than the highWaterMark, then also read some
               if (state.length === 0 || state.length - n < state.highWaterMark) {
                  doRead = true;
                  debug('length less than watermark', doRead);
               }

               // however, if we've ended, then there's no point, and if we're already
               // reading, then it's unnecessary.
               if (state.ended || state.reading) {
                  doRead = false;
                  debug('reading or ended', doRead);
               } else if (doRead) {
                  debug('do read');
                  state.reading = true;
                  state.sync = true;
                  // if the length is currently zero, then we *need* a readable event.
                  if (state.length === 0) state.needReadable = true;
                  // call internal read method
                  this._read(state.highWaterMark);
                  state.sync = false;
                  // If _read pushed data synchronously, then `reading` will be false,
                  // and we need to re-evaluate how much data we can return to the user.
                  if (!state.reading) n = howMuchToRead(nOrig, state);
               }

               var ret;
               if (n > 0) ret = fromList(n, state); else ret = null;

               if (ret === null) {
                  state.needReadable = true;
                  n = 0;
               } else {
                  state.length -= n;
               }

               if (state.length === 0) {
                  // If we have nothing in the buffer, then we want to know
                  // as soon as we *do* get something into the buffer.
                  if (!state.ended) state.needReadable = true;

                  // If we tried to read() past the EOF, then emit end on the next tick.
                  if (nOrig !== n && state.ended) endReadable(this);
               }

               if (ret !== null) this.emit('data', ret);

               return ret;
            };

            function onEofChunk(stream, state) {
               if (state.ended) return;
               if (state.decoder) {
                  var chunk = state.decoder.end();
                  if (chunk && chunk.length) {
                     state.buffer.push(chunk);
                     state.length += state.objectMode ? 1 : chunk.length;
                  }
               }
               state.ended = true;

               // emit 'readable' now to make sure it gets picked up.
               emitReadable(stream);
            }

            // Don't emit readable right away in sync mode, because this can trigger
            // another read() call => stack overflow.  This way, it might trigger
            // a nextTick recursion warning, but that's not so bad.
            function emitReadable(stream) {
               var state = stream._readableState;
               state.needReadable = false;
               if (!state.emittedReadable) {
                  debug('emitReadable', state.flowing);
                  state.emittedReadable = true;
                  if (state.sync) processNextTick(emitReadable_, stream); else emitReadable_(stream);
               }
            }

            function emitReadable_(stream) {
               debug('emit readable');
               stream.emit('readable');
               flow(stream);
            }

            // at this point, the user has presumably seen the 'readable' event,
            // and called read() to consume some data.  that may have triggered
            // in turn another _read(n) call, in which case reading = true if
            // it's in progress.
            // However, if we're not ended, or reading, and the length < hwm,
            // then go ahead and try to read some more preemptively.
            function maybeReadMore(stream, state) {
               if (!state.readingMore) {
                  state.readingMore = true;
                  processNextTick(maybeReadMore_, stream, state);
               }
            }

            function maybeReadMore_(stream, state) {
               var len = state.length;
               while (!state.reading && !state.flowing && !state.ended && state.length < state.highWaterMark) {
                  debug('maybeReadMore read 0');
                  stream.read(0);
                  if (len === state.length)
                     // didn't get any data, stop spinning.
                     break; else len = state.length;
               }
               state.readingMore = false;
            }

            // abstract method.  to be overridden in specific implementation classes.
            // call cb(er, data) where data is <= n in length.
            // for virtual (non-string, non-buffer) streams, "length" is somewhat
            // arbitrary, and perhaps not very meaningful.
            Readable.prototype._read = function (n) {
               this.emit('error', new Error('_read() is not implemented'));
            };

            Readable.prototype.pipe = function (dest, pipeOpts) {
               var src = this;
               var state = this._readableState;

               switch (state.pipesCount) {
                  case 0:
                     state.pipes = dest;
                     break;
                  case 1:
                     state.pipes = [state.pipes, dest];
                     break;
                  default:
                     state.pipes.push(dest);
                     break;
               }
               state.pipesCount += 1;
               debug('pipe count=%d opts=%j', state.pipesCount, pipeOpts);

               var doEnd = (!pipeOpts || pipeOpts.end !== false) && dest !== process.stdout && dest !== process.stderr;

               var endFn = doEnd ? onend : unpipe;
               if (state.endEmitted) processNextTick(endFn); else src.once('end', endFn);

               dest.on('unpipe', onunpipe);
               function onunpipe(readable, unpipeInfo) {
                  debug('onunpipe');
                  if (readable === src) {
                     if (unpipeInfo && unpipeInfo.hasUnpiped === false) {
                        unpipeInfo.hasUnpiped = true;
                        cleanup();
                     }
                  }
               }

               function onend() {
                  debug('onend');
                  dest.end();
               }

               // when the dest drains, it reduces the awaitDrain counter
               // on the source.  This would be more elegant with a .once()
               // handler in flow(), but adding and removing repeatedly is
               // too slow.
               var ondrain = pipeOnDrain(src);
               dest.on('drain', ondrain);

               var cleanedUp = false;
               function cleanup() {
                  debug('cleanup');
                  // cleanup event handlers once the pipe is broken
                  dest.removeListener('close', onclose);
                  dest.removeListener('finish', onfinish);
                  dest.removeListener('drain', ondrain);
                  dest.removeListener('error', onerror);
                  dest.removeListener('unpipe', onunpipe);
                  src.removeListener('end', onend);
                  src.removeListener('end', unpipe);
                  src.removeListener('data', ondata);

                  cleanedUp = true;

                  // if the reader is waiting for a drain event from this
                  // specific writer, then it would cause it to never start
                  // flowing again.
                  // So, if this is awaiting a drain, then we just call it now.
                  // If we don't know, then assume that we are waiting for one.
                  if (state.awaitDrain && (!dest._writableState || dest._writableState.needDrain)) ondrain();
               }

               // If the user pushes more data while we're writing to dest then we'll end up
               // in ondata again. However, we only want to increase awaitDrain once because
               // dest will only emit one 'drain' event for the multiple writes.
               // => Introduce a guard on increasing awaitDrain.
               var increasedAwaitDrain = false;
               src.on('data', ondata);
               function ondata(chunk) {
                  debug('ondata');
                  increasedAwaitDrain = false;
                  var ret = dest.write(chunk);
                  if (false === ret && !increasedAwaitDrain) {
                     // If the user unpiped during `dest.write()`, it is possible
                     // to get stuck in a permanently paused state if that write
                     // also returned false.
                     // => Check whether `dest` is still a piping destination.
                     if ((state.pipesCount === 1 && state.pipes === dest || state.pipesCount > 1 && indexOf(state.pipes, dest) !== -1) && !cleanedUp) {
                        debug('false write response, pause', src._readableState.awaitDrain);
                        src._readableState.awaitDrain++;
                        increasedAwaitDrain = true;
                     }
                     src.pause();
                  }
               }

               // if the dest has an error, then stop piping into it.
               // however, don't suppress the throwing behavior for this.
               function onerror(er) {
                  debug('onerror', er);
                  unpipe();
                  dest.removeListener('error', onerror);
                  if (EElistenerCount(dest, 'error') === 0) dest.emit('error', er);
               }

               // Make sure our error handler is attached before userland ones.
               prependListener(dest, 'error', onerror);

               // Both close and finish should trigger unpipe, but only once.
               function onclose() {
                  dest.removeListener('finish', onfinish);
                  unpipe();
               }
               dest.once('close', onclose);
               function onfinish() {
                  debug('onfinish');
                  dest.removeListener('close', onclose);
                  unpipe();
               }
               dest.once('finish', onfinish);

               function unpipe() {
                  debug('unpipe');
                  src.unpipe(dest);
               }

               // tell the dest that it's being piped to
               dest.emit('pipe', src);

               // start the flow if it hasn't been started already.
               if (!state.flowing) {
                  debug('pipe resume');
                  src.resume();
               }

               return dest;
            };

            function pipeOnDrain(src) {
               return function () {
                  var state = src._readableState;
                  debug('pipeOnDrain', state.awaitDrain);
                  if (state.awaitDrain) state.awaitDrain--;
                  if (state.awaitDrain === 0 && EElistenerCount(src, 'data')) {
                     state.flowing = true;
                     flow(src);
                  }
               };
            }

            Readable.prototype.unpipe = function (dest) {
               var state = this._readableState;
               var unpipeInfo = { hasUnpiped: false };

               // if we're not piping anywhere, then do nothing.
               if (state.pipesCount === 0) return this;

               // just one destination.  most common case.
               if (state.pipesCount === 1) {
                  // passed in one, but it's not the right one.
                  if (dest && dest !== state.pipes) return this;

                  if (!dest) dest = state.pipes;

                  // got a match.
                  state.pipes = null;
                  state.pipesCount = 0;
                  state.flowing = false;
                  if (dest) dest.emit('unpipe', this, unpipeInfo);
                  return this;
               }

               // slow case. multiple pipe destinations.

               if (!dest) {
                  // remove all.
                  var dests = state.pipes;
                  var len = state.pipesCount;
                  state.pipes = null;
                  state.pipesCount = 0;
                  state.flowing = false;

                  for (var i = 0; i < len; i++) {
                     dests[i].emit('unpipe', this, unpipeInfo);
                  } return this;
               }

               // try to find the right one.
               var index = indexOf(state.pipes, dest);
               if (index === -1) return this;

               state.pipes.splice(index, 1);
               state.pipesCount -= 1;
               if (state.pipesCount === 1) state.pipes = state.pipes[0];

               dest.emit('unpipe', this, unpipeInfo);

               return this;
            };

            // set up data events if they are asked for
            // Ensure readable listeners eventually get something
            Readable.prototype.on = function (ev, fn) {
               var res = Stream.prototype.on.call(this, ev, fn);

               if (ev === 'data') {
                  // Start flowing on next tick if stream isn't explicitly paused
                  if (this._readableState.flowing !== false) this.resume();
               } else if (ev === 'readable') {
                  var state = this._readableState;
                  if (!state.endEmitted && !state.readableListening) {
                     state.readableListening = state.needReadable = true;
                     state.emittedReadable = false;
                     if (!state.reading) {
                        processNextTick(nReadingNextTick, this);
                     } else if (state.length) {
                        emitReadable(this);
                     }
                  }
               }

               return res;
            };
            Readable.prototype.addListener = Readable.prototype.on;

            function nReadingNextTick(self) {
               debug('readable nexttick read 0');
               self.read(0);
            }

            // pause() and resume() are remnants of the legacy readable stream API
            // If the user uses them, then switch into old mode.
            Readable.prototype.resume = function () {
               var state = this._readableState;
               if (!state.flowing) {
                  debug('resume');
                  state.flowing = true;
                  resume(this, state);
               }
               return this;
            };

            function resume(stream, state) {
               if (!state.resumeScheduled) {
                  state.resumeScheduled = true;
                  processNextTick(resume_, stream, state);
               }
            }

            function resume_(stream, state) {
               if (!state.reading) {
                  debug('resume read 0');
                  stream.read(0);
               }

               state.resumeScheduled = false;
               state.awaitDrain = 0;
               stream.emit('resume');
               flow(stream);
               if (state.flowing && !state.reading) stream.read(0);
            }

            Readable.prototype.pause = function () {
               debug('call pause flowing=%j', this._readableState.flowing);
               if (false !== this._readableState.flowing) {
                  debug('pause');
                  this._readableState.flowing = false;
                  this.emit('pause');
               }
               return this;
            };

            function flow(stream) {
               var state = stream._readableState;
               debug('flow', state.flowing);
               while (state.flowing && stream.read() !== null) { }
            }

            // wrap an old-style stream as the async data source.
            // This is *not* part of the readable stream interface.
            // It is an ugly unfortunate mess of history.
            Readable.prototype.wrap = function (stream) {
               var state = this._readableState;
               var paused = false;

               var self = this;
               stream.on('end', function () {
                  debug('wrapped end');
                  if (state.decoder && !state.ended) {
                     var chunk = state.decoder.end();
                     if (chunk && chunk.length) self.push(chunk);
                  }

                  self.push(null);
               });

               stream.on('data', function (chunk) {
                  debug('wrapped data');
                  if (state.decoder) chunk = state.decoder.write(chunk);

                  // don't skip over falsy values in objectMode
                  if (state.objectMode && (chunk === null || chunk === undefined)) return; else if (!state.objectMode && (!chunk || !chunk.length)) return;

                  var ret = self.push(chunk);
                  if (!ret) {
                     paused = true;
                     stream.pause();
                  }
               });

               // proxy all the other methods.
               // important when wrapping filters and duplexes.
               for (var i in stream) {
                  if (this[i] === undefined && typeof stream[i] === 'function') {
                     this[i] = function (method) {
                        return function () {
                           return stream[method].apply(stream, arguments);
                        };
                     }(i);
                  }
               }

               // proxy certain important events.
               for (var n = 0; n < kProxyEvents.length; n++) {
                  stream.on(kProxyEvents[n], self.emit.bind(self, kProxyEvents[n]));
               }

               // when we try to consume some more bytes, simply unpause the
               // underlying stream.
               self._read = function (n) {
                  debug('wrapped _read', n);
                  if (paused) {
                     paused = false;
                     stream.resume();
                  }
               };

               return self;
            };

            // exposed for testing purposes only.
            Readable._fromList = fromList;

            // Pluck off n bytes from an array of buffers.
            // Length is the combined lengths of all the buffers in the list.
            // This function is designed to be inlinable, so please take care when making
            // changes to the function body.
            function fromList(n, state) {
               // nothing buffered
               if (state.length === 0) return null;

               var ret;
               if (state.objectMode) ret = state.buffer.shift(); else if (!n || n >= state.length) {
                  // read it all, truncate the list
                  if (state.decoder) ret = state.buffer.join(''); else if (state.buffer.length === 1) ret = state.buffer.head.data; else ret = state.buffer.concat(state.length);
                  state.buffer.clear();
               } else {
                  // read part of list
                  ret = fromListPartial(n, state.buffer, state.decoder);
               }

               return ret;
            }

            // Extracts only enough buffered data to satisfy the amount requested.
            // This function is designed to be inlinable, so please take care when making
            // changes to the function body.
            function fromListPartial(n, list, hasStrings) {
               var ret;
               if (n < list.head.data.length) {
                  // slice is the same for buffers and strings
                  ret = list.head.data.slice(0, n);
                  list.head.data = list.head.data.slice(n);
               } else if (n === list.head.data.length) {
                  // first chunk is a perfect match
                  ret = list.shift();
               } else {
                  // result spans more than one buffer
                  ret = hasStrings ? copyFromBufferString(n, list) : copyFromBuffer(n, list);
               }
               return ret;
            }

            // Copies a specified amount of characters from the list of buffered data
            // chunks.
            // This function is designed to be inlinable, so please take care when making
            // changes to the function body.
            function copyFromBufferString(n, list) {
               var p = list.head;
               var c = 1;
               var ret = p.data;
               n -= ret.length;
               while (p = p.next) {
                  var str = p.data;
                  var nb = n > str.length ? str.length : n;
                  if (nb === str.length) ret += str; else ret += str.slice(0, n);
                  n -= nb;
                  if (n === 0) {
                     if (nb === str.length) {
                        ++c;
                        if (p.next) list.head = p.next; else list.head = list.tail = null;
                     } else {
                        list.head = p;
                        p.data = str.slice(nb);
                     }
                     break;
                  }
                  ++c;
               }
               list.length -= c;
               return ret;
            }

            // Copies a specified amount of bytes from the list of buffered data chunks.
            // This function is designed to be inlinable, so please take care when making
            // changes to the function body.
            function copyFromBuffer(n, list) {
               var ret = Buffer.allocUnsafe(n);
               var p = list.head;
               var c = 1;
               p.data.copy(ret);
               n -= p.data.length;
               while (p = p.next) {
                  var buf = p.data;
                  var nb = n > buf.length ? buf.length : n;
                  buf.copy(ret, ret.length - n, 0, nb);
                  n -= nb;
                  if (n === 0) {
                     if (nb === buf.length) {
                        ++c;
                        if (p.next) list.head = p.next; else list.head = list.tail = null;
                     } else {
                        list.head = p;
                        p.data = buf.slice(nb);
                     }
                     break;
                  }
                  ++c;
               }
               list.length -= c;
               return ret;
            }

            function endReadable(stream) {
               var state = stream._readableState;

               // If we get here before consuming all the bytes, then that is a
               // bug in node.  Should never happen.
               if (state.length > 0) throw new Error('"endReadable()" called on non-empty stream');

               if (!state.endEmitted) {
                  state.ended = true;
                  processNextTick(endReadableNT, state, stream);
               }
            }

            function endReadableNT(state, stream) {
               // Check that we didn't get one last unshift.
               if (!state.endEmitted && state.length === 0) {
                  state.endEmitted = true;
                  stream.readable = false;
                  stream.emit('end');
               }
            }

            function forEach(xs, f) {
               for (var i = 0, l = xs.length; i < l; i++) {
                  f(xs[i], i);
               }
            }

            function indexOf(xs, x) {
               for (var i = 0, l = xs.length; i < l; i++) {
                  if (xs[i] === x) return i;
               }
               return -1;
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1), __webpack_require__(6)))

         /***/
      }),
/* 23 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
         // Copyright Joyent, Inc. and other Node contributors.
         //
         // Permission is hereby granted, free of charge, to any person obtaining a
         // copy of this software and associated documentation files (the
         // "Software"), to deal in the Software without restriction, including
         // without limitation the rights to use, copy, modify, merge, publish,
         // distribute, sublicense, and/or sell copies of the Software, and to permit
         // persons to whom the Software is furnished to do so, subject to the
         // following conditions:
         //
         // The above copyright notice and this permission notice shall be included
         // in all copies or substantial portions of the Software.
         //
         // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
         // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
         // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
         // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
         // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
         // USE OR OTHER DEALINGS IN THE SOFTWARE.

         // a transform stream is a readable/writable stream where you do
         // something with the data.  Sometimes it's called a "filter",
         // but that's not a great name for it, since that implies a thing where
         // some bits pass through, and others are simply ignored.  (That would
         // be a valid example of a transform, of course.)
         //
         // While the output is causally related to the input, it's not a
         // necessarily symmetric or synchronous transformation.  For example,
         // a zlib stream might take multiple plain-text writes(), and then
         // emit a single compressed chunk some time in the future.
         //
         // Here's how this works:
         //
         // The Transform stream has all the aspects of the readable and writable
         // stream classes.  When you write(chunk), that calls _write(chunk,cb)
         // internally, and returns false if there's a lot of pending writes
         // buffered up.  When you call read(), that calls _read(n) until
         // there's enough pending readable data buffered up.
         //
         // In a transform stream, the written data is placed in a buffer.  When
         // _read(n) is called, it transforms the queued up data, calling the
         // buffered _write cb's as it consumes chunks.  If consuming a single
         // written chunk would result in multiple output chunks, then the first
         // outputted bit calls the readcb, and subsequent chunks just go into
         // the read buffer, and will cause it to emit 'readable' if necessary.
         //
         // This way, back-pressure is actually determined by the reading side,
         // since _read has to be called to start processing a new chunk.  However,
         // a pathological inflate type of transform can cause excessive buffering
         // here.  For example, imagine a stream where every byte of input is
         // interpreted as an integer from 0-255, and then results in that many
         // bytes of output.  Writing the 4 bytes {ff,ff,ff,ff} would result in
         // 1kb of data being output.  In this case, you could write a very small
         // amount of input, and end up with a very large amount of output.  In
         // such a pathological inflating mechanism, there'd be no way to tell
         // the system to stop doing the transform.  A single 4MB write could
         // cause the system to run out of memory.
         //
         // However, even in such a pathological case, only a single written chunk
         // would be consumed, and then the rest would wait (un-transformed) until
         // the results of the previous transformed chunk were consumed.



         module.exports = Transform;

         var Duplex = __webpack_require__(0);

         /*<replacement>*/
         var util = __webpack_require__(5);
         util.inherits = __webpack_require__(2);
         /*</replacement>*/

         util.inherits(Transform, Duplex);

         function TransformState(stream) {
            this.afterTransform = function (er, data) {
               return afterTransform(stream, er, data);
            };

            this.needTransform = false;
            this.transforming = false;
            this.writecb = null;
            this.writechunk = null;
            this.writeencoding = null;
         }

         function afterTransform(stream, er, data) {
            var ts = stream._transformState;
            ts.transforming = false;

            var cb = ts.writecb;

            if (!cb) {
               return stream.emit('error', new Error('write callback called multiple times'));
            }

            ts.writechunk = null;
            ts.writecb = null;

            if (data !== null && data !== undefined) stream.push(data);

            cb(er);

            var rs = stream._readableState;
            rs.reading = false;
            if (rs.needReadable || rs.length < rs.highWaterMark) {
               stream._read(rs.highWaterMark);
            }
         }

         function Transform(options) {
            if (!(this instanceof Transform)) return new Transform(options);

            Duplex.call(this, options);

            this._transformState = new TransformState(this);

            var stream = this;

            // start out asking for a readable event once data is transformed.
            this._readableState.needReadable = true;

            // we have implemented the _read method, and done the other things
            // that Readable wants before the first _read call, so unset the
            // sync guard flag.
            this._readableState.sync = false;

            if (options) {
               if (typeof options.transform === 'function') this._transform = options.transform;

               if (typeof options.flush === 'function') this._flush = options.flush;
            }

            // When the writable side finishes, then flush out anything remaining.
            this.once('prefinish', function () {
               if (typeof this._flush === 'function') this._flush(function (er, data) {
                  done(stream, er, data);
               }); else done(stream);
            });
         }

         Transform.prototype.push = function (chunk, encoding) {
            this._transformState.needTransform = false;
            return Duplex.prototype.push.call(this, chunk, encoding);
         };

         // This is the part where you do stuff!
         // override this function in implementation classes.
         // 'chunk' is an input chunk.
         //
         // Call `push(newChunk)` to pass along transformed output
         // to the readable side.  You may call 'push' zero or more times.
         //
         // Call `cb(err)` when you are done with this chunk.  If you pass
         // an error, then that'll put the hurt on the whole operation.  If you
         // never call cb(), then you'll never get another chunk.
         Transform.prototype._transform = function (chunk, encoding, cb) {
            throw new Error('_transform() is not implemented');
         };

         Transform.prototype._write = function (chunk, encoding, cb) {
            var ts = this._transformState;
            ts.writecb = cb;
            ts.writechunk = chunk;
            ts.writeencoding = encoding;
            if (!ts.transforming) {
               var rs = this._readableState;
               if (ts.needTransform || rs.needReadable || rs.length < rs.highWaterMark) this._read(rs.highWaterMark);
            }
         };

         // Doesn't matter what the args are here.
         // _transform does all the work.
         // That we got here means that the readable side wants more data.
         Transform.prototype._read = function (n) {
            var ts = this._transformState;

            if (ts.writechunk !== null && ts.writecb && !ts.transforming) {
               ts.transforming = true;
               this._transform(ts.writechunk, ts.writeencoding, ts.afterTransform);
            } else {
               // mark that we need a transform, so that any data that comes in
               // will get processed, now that we've asked for it.
               ts.needTransform = true;
            }
         };

         Transform.prototype._destroy = function (err, cb) {
            var _this = this;

            Duplex.prototype._destroy.call(this, err, function (err2) {
               cb(err2);
               _this.emit('close');
            });
         };

         function done(stream, er, data) {
            if (er) return stream.emit('error', er);

            if (data !== null && data !== undefined) stream.push(data);

            // if there's nothing in the write buffer, then that means
            // that nothing more will ever be provided
            var ws = stream._writableState;
            var ts = stream._transformState;

            if (ws.length) throw new Error('Calling transform done when ws.length != 0');

            if (ts.transforming) throw new Error('Calling transform done when still transforming');

            return stream.push(null);
         }

         /***/
      }),
/* 24 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (process, setImmediate, global) {// Copyright Joyent, Inc. and other Node contributors.
            //
            // Permission is hereby granted, free of charge, to any person obtaining a
            // copy of this software and associated documentation files (the
            // "Software"), to deal in the Software without restriction, including
            // without limitation the rights to use, copy, modify, merge, publish,
            // distribute, sublicense, and/or sell copies of the Software, and to permit
            // persons to whom the Software is furnished to do so, subject to the
            // following conditions:
            //
            // The above copyright notice and this permission notice shall be included
            // in all copies or substantial portions of the Software.
            //
            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
            // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
            // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
            // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
            // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
            // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
            // USE OR OTHER DEALINGS IN THE SOFTWARE.

            // A bit simpler than readable streams.
            // Implement an async ._write(chunk, encoding, cb), and it'll handle all
            // the drain event emission and buffering.



            /*<replacement>*/

            var processNextTick = __webpack_require__(10);
            /*</replacement>*/

            module.exports = Writable;

            /* <replacement> */
            function WriteReq(chunk, encoding, cb) {
               this.chunk = chunk;
               this.encoding = encoding;
               this.callback = cb;
               this.next = null;
            }

            // It seems a linked list but it is not
            // there will be only 2 of these for each stream
            function CorkedRequest(state) {
               var _this = this;

               this.next = null;
               this.entry = null;
               this.finish = function () {
                  onCorkedFinish(_this, state);
               };
            }
            /* </replacement> */

            /*<replacement>*/
            var asyncWrite = !process.browser && ['v0.10', 'v0.9.'].indexOf(process.version.slice(0, 5)) > -1 ? setImmediate : processNextTick;
            /*</replacement>*/

            /*<replacement>*/
            var Duplex;
            /*</replacement>*/

            Writable.WritableState = WritableState;

            /*<replacement>*/
            var util = __webpack_require__(5);
            util.inherits = __webpack_require__(2);
            /*</replacement>*/

            /*<replacement>*/
            var internalUtil = {
               deprecate: __webpack_require__(47)
            };
            /*</replacement>*/

            /*<replacement>*/
            var Stream = __webpack_require__(26);
            /*</replacement>*/

            /*<replacement>*/
            var Buffer = __webpack_require__(7).Buffer;
            var OurUint8Array = global.Uint8Array || function () { };
            function _uint8ArrayToBuffer(chunk) {
               return Buffer.from(chunk);
            }
            function _isUint8Array(obj) {
               return Buffer.isBuffer(obj) || obj instanceof OurUint8Array;
            }
            /*</replacement>*/

            var destroyImpl = __webpack_require__(25);

            util.inherits(Writable, Stream);

            function nop() { }

            function WritableState(options, stream) {
               Duplex = Duplex || __webpack_require__(0);

               options = options || {};

               // object stream flag to indicate whether or not this stream
               // contains buffers or objects.
               this.objectMode = !!options.objectMode;

               if (stream instanceof Duplex) this.objectMode = this.objectMode || !!options.writableObjectMode;

               // the point at which write() starts returning false
               // Note: 0 is a valid value, means that we always return false if
               // the entire buffer is not flushed immediately on write()
               var hwm = options.highWaterMark;
               var defaultHwm = this.objectMode ? 16 : 16 * 1024;
               this.highWaterMark = hwm || hwm === 0 ? hwm : defaultHwm;

               // cast to ints.
               this.highWaterMark = Math.floor(this.highWaterMark);

               // if _final has been called
               this.finalCalled = false;

               // drain event flag.
               this.needDrain = false;
               // at the start of calling end()
               this.ending = false;
               // when end() has been called, and returned
               this.ended = false;
               // when 'finish' is emitted
               this.finished = false;

               // has it been destroyed
               this.destroyed = false;

               // should we decode strings into buffers before passing to _write?
               // this is here so that some node-core streams can optimize string
               // handling at a lower level.
               var noDecode = options.decodeStrings === false;
               this.decodeStrings = !noDecode;

               // Crypto is kind of old and crusty.  Historically, its default string
               // encoding is 'binary' so we have to make this configurable.
               // Everything else in the universe uses 'utf8', though.
               this.defaultEncoding = options.defaultEncoding || 'utf8';

               // not an actual buffer we keep track of, but a measurement
               // of how much we're waiting to get pushed to some underlying
               // socket or file.
               this.length = 0;

               // a flag to see when we're in the middle of a write.
               this.writing = false;

               // when true all writes will be buffered until .uncork() call
               this.corked = 0;

               // a flag to be able to tell if the onwrite cb is called immediately,
               // or on a later tick.  We set this to true at first, because any
               // actions that shouldn't happen until "later" should generally also
               // not happen before the first write call.
               this.sync = true;

               // a flag to know if we're processing previously buffered items, which
               // may call the _write() callback in the same tick, so that we don't
               // end up in an overlapped onwrite situation.
               this.bufferProcessing = false;

               // the callback that's passed to _write(chunk,cb)
               this.onwrite = function (er) {
                  onwrite(stream, er);
               };

               // the callback that the user supplies to write(chunk,encoding,cb)
               this.writecb = null;

               // the amount that is being written when _write is called.
               this.writelen = 0;

               this.bufferedRequest = null;
               this.lastBufferedRequest = null;

               // number of pending user-supplied write callbacks
               // this must be 0 before 'finish' can be emitted
               this.pendingcb = 0;

               // emit prefinish if the only thing we're waiting for is _write cbs
               // This is relevant for synchronous Transform streams
               this.prefinished = false;

               // True if the error was already emitted and should not be thrown again
               this.errorEmitted = false;

               // count buffered requests
               this.bufferedRequestCount = 0;

               // allocate the first CorkedRequest, there is always
               // one allocated and free to use, and we maintain at most two
               this.corkedRequestsFree = new CorkedRequest(this);
            }

            WritableState.prototype.getBuffer = function getBuffer() {
               var current = this.bufferedRequest;
               var out = [];
               while (current) {
                  out.push(current);
                  current = current.next;
               }
               return out;
            };

            (function () {
               try {
                  Object.defineProperty(WritableState.prototype, 'buffer', {
                     get: internalUtil.deprecate(function () {
                        return this.getBuffer();
                     }, '_writableState.buffer is deprecated. Use _writableState.getBuffer ' + 'instead.', 'DEP0003')
                  });
               } catch (_) { }
            })();

            // Test _writableState for inheritance to account for Duplex streams,
            // whose prototype chain only points to Readable.
            var realHasInstance;
            if (typeof Symbol === 'function' && Symbol.hasInstance && typeof Function.prototype[Symbol.hasInstance] === 'function') {
               realHasInstance = Function.prototype[Symbol.hasInstance];
               Object.defineProperty(Writable, Symbol.hasInstance, {
                  value: function value(object) {
                     if (realHasInstance.call(this, object)) return true;

                     return object && object._writableState instanceof WritableState;
                  }
               });
            } else {
               realHasInstance = function realHasInstance(object) {
                  return object instanceof this;
               };
            }

            function Writable(options) {
               Duplex = Duplex || __webpack_require__(0);

               // Writable ctor is applied to Duplexes, too.
               // `realHasInstance` is necessary because using plain `instanceof`
               // would return false, as no `_writableState` property is attached.

               // Trying to use the custom `instanceof` for Writable here will also break the
               // Node.js LazyTransform implementation, which has a non-trivial getter for
               // `_writableState` that would lead to infinite recursion.
               if (!realHasInstance.call(Writable, this) && !(this instanceof Duplex)) {
                  return new Writable(options);
               }

               this._writableState = new WritableState(options, this);

               // legacy.
               this.writable = true;

               if (options) {
                  if (typeof options.write === 'function') this._write = options.write;

                  if (typeof options.writev === 'function') this._writev = options.writev;

                  if (typeof options.destroy === 'function') this._destroy = options.destroy;

                  if (typeof options.final === 'function') this._final = options.final;
               }

               Stream.call(this);
            }

            // Otherwise people can pipe Writable streams, which is just wrong.
            Writable.prototype.pipe = function () {
               this.emit('error', new Error('Cannot pipe, not readable'));
            };

            function writeAfterEnd(stream, cb) {
               var er = new Error('write after end');
               // TODO: defer error events consistently everywhere, not just the cb
               stream.emit('error', er);
               processNextTick(cb, er);
            }

            // Checks that a user-supplied chunk is valid, especially for the particular
            // mode the stream is in. Currently this means that `null` is never accepted
            // and undefined/non-string values are only allowed in object mode.
            function validChunk(stream, state, chunk, cb) {
               var valid = true;
               var er = false;

               if (chunk === null) {
                  er = new TypeError('May not write null values to stream');
               } else if (typeof chunk !== 'string' && chunk !== undefined && !state.objectMode) {
                  er = new TypeError('Invalid non-string/buffer chunk');
               }
               if (er) {
                  stream.emit('error', er);
                  processNextTick(cb, er);
                  valid = false;
               }
               return valid;
            }

            Writable.prototype.write = function (chunk, encoding, cb) {
               var state = this._writableState;
               var ret = false;
               var isBuf = _isUint8Array(chunk) && !state.objectMode;

               if (isBuf && !Buffer.isBuffer(chunk)) {
                  chunk = _uint8ArrayToBuffer(chunk);
               }

               if (typeof encoding === 'function') {
                  cb = encoding;
                  encoding = null;
               }

               if (isBuf) encoding = 'buffer'; else if (!encoding) encoding = state.defaultEncoding;

               if (typeof cb !== 'function') cb = nop;

               if (state.ended) writeAfterEnd(this, cb); else if (isBuf || validChunk(this, state, chunk, cb)) {
                  state.pendingcb++;
                  ret = writeOrBuffer(this, state, isBuf, chunk, encoding, cb);
               }

               return ret;
            };

            Writable.prototype.cork = function () {
               var state = this._writableState;

               state.corked++;
            };

            Writable.prototype.uncork = function () {
               var state = this._writableState;

               if (state.corked) {
                  state.corked--;

                  if (!state.writing && !state.corked && !state.finished && !state.bufferProcessing && state.bufferedRequest) clearBuffer(this, state);
               }
            };

            Writable.prototype.setDefaultEncoding = function setDefaultEncoding(encoding) {
               // node::ParseEncoding() requires lower case.
               if (typeof encoding === 'string') encoding = encoding.toLowerCase();
               if (!(['hex', 'utf8', 'utf-8', 'ascii', 'binary', 'base64', 'ucs2', 'ucs-2', 'utf16le', 'utf-16le', 'raw'].indexOf((encoding + '').toLowerCase()) > -1)) throw new TypeError('Unknown encoding: ' + encoding);
               this._writableState.defaultEncoding = encoding;
               return this;
            };

            function decodeChunk(state, chunk, encoding) {
               if (!state.objectMode && state.decodeStrings !== false && typeof chunk === 'string') {
                  chunk = Buffer.from(chunk, encoding);
               }
               return chunk;
            }

            // if we're already writing something, then just put this
            // in the queue, and wait our turn.  Otherwise, call _write
            // If we return false, then we need a drain event, so set that flag.
            function writeOrBuffer(stream, state, isBuf, chunk, encoding, cb) {
               if (!isBuf) {
                  var newChunk = decodeChunk(state, chunk, encoding);
                  if (chunk !== newChunk) {
                     isBuf = true;
                     encoding = 'buffer';
                     chunk = newChunk;
                  }
               }
               var len = state.objectMode ? 1 : chunk.length;

               state.length += len;

               var ret = state.length < state.highWaterMark;
               // we must ensure that previous needDrain will not be reset to false.
               if (!ret) state.needDrain = true;

               if (state.writing || state.corked) {
                  var last = state.lastBufferedRequest;
                  state.lastBufferedRequest = {
                     chunk: chunk,
                     encoding: encoding,
                     isBuf: isBuf,
                     callback: cb,
                     next: null
                  };
                  if (last) {
                     last.next = state.lastBufferedRequest;
                  } else {
                     state.bufferedRequest = state.lastBufferedRequest;
                  }
                  state.bufferedRequestCount += 1;
               } else {
                  doWrite(stream, state, false, len, chunk, encoding, cb);
               }

               return ret;
            }

            function doWrite(stream, state, writev, len, chunk, encoding, cb) {
               state.writelen = len;
               state.writecb = cb;
               state.writing = true;
               state.sync = true;
               if (writev) stream._writev(chunk, state.onwrite); else stream._write(chunk, encoding, state.onwrite);
               state.sync = false;
            }

            function onwriteError(stream, state, sync, er, cb) {
               --state.pendingcb;

               if (sync) {
                  // defer the callback if we are being called synchronously
                  // to avoid piling up things on the stack
                  processNextTick(cb, er);
                  // this can emit finish, and it will always happen
                  // after error
                  processNextTick(finishMaybe, stream, state);
                  stream._writableState.errorEmitted = true;
                  stream.emit('error', er);
               } else {
                  // the caller expect this to happen before if
                  // it is async
                  cb(er);
                  stream._writableState.errorEmitted = true;
                  stream.emit('error', er);
                  // this can emit finish, but finish must
                  // always follow error
                  finishMaybe(stream, state);
               }
            }

            function onwriteStateUpdate(state) {
               state.writing = false;
               state.writecb = null;
               state.length -= state.writelen;
               state.writelen = 0;
            }

            function onwrite(stream, er) {
               var state = stream._writableState;
               var sync = state.sync;
               var cb = state.writecb;

               onwriteStateUpdate(state);

               if (er) onwriteError(stream, state, sync, er, cb); else {
                  // Check if we're actually ready to finish, but don't emit yet
                  var finished = needFinish(state);

                  if (!finished && !state.corked && !state.bufferProcessing && state.bufferedRequest) {
                     clearBuffer(stream, state);
                  }

                  if (sync) {
                     /*<replacement>*/
                     asyncWrite(afterWrite, stream, state, finished, cb);
                     /*</replacement>*/
                  } else {
                     afterWrite(stream, state, finished, cb);
                  }
               }
            }

            function afterWrite(stream, state, finished, cb) {
               if (!finished) onwriteDrain(stream, state);
               state.pendingcb--;
               cb();
               finishMaybe(stream, state);
            }

            // Must force callback to be called on nextTick, so that we don't
            // emit 'drain' before the write() consumer gets the 'false' return
            // value, and has a chance to attach a 'drain' listener.
            function onwriteDrain(stream, state) {
               if (state.length === 0 && state.needDrain) {
                  state.needDrain = false;
                  stream.emit('drain');
               }
            }

            // if there's something in the buffer waiting, then process it
            function clearBuffer(stream, state) {
               state.bufferProcessing = true;
               var entry = state.bufferedRequest;

               if (stream._writev && entry && entry.next) {
                  // Fast case, write everything using _writev()
                  var l = state.bufferedRequestCount;
                  var buffer = new Array(l);
                  var holder = state.corkedRequestsFree;
                  holder.entry = entry;

                  var count = 0;
                  var allBuffers = true;
                  while (entry) {
                     buffer[count] = entry;
                     if (!entry.isBuf) allBuffers = false;
                     entry = entry.next;
                     count += 1;
                  }
                  buffer.allBuffers = allBuffers;

                  doWrite(stream, state, true, state.length, buffer, '', holder.finish);

                  // doWrite is almost always async, defer these to save a bit of time
                  // as the hot path ends with doWrite
                  state.pendingcb++;
                  state.lastBufferedRequest = null;
                  if (holder.next) {
                     state.corkedRequestsFree = holder.next;
                     holder.next = null;
                  } else {
                     state.corkedRequestsFree = new CorkedRequest(state);
                  }
               } else {
                  // Slow case, write chunks one-by-one
                  while (entry) {
                     var chunk = entry.chunk;
                     var encoding = entry.encoding;
                     var cb = entry.callback;
                     var len = state.objectMode ? 1 : chunk.length;

                     doWrite(stream, state, false, len, chunk, encoding, cb);
                     entry = entry.next;
                     // if we didn't call the onwrite immediately, then
                     // it means that we need to wait until it does.
                     // also, that means that the chunk and cb are currently
                     // being processed, so move the buffer counter past them.
                     if (state.writing) {
                        break;
                     }
                  }

                  if (entry === null) state.lastBufferedRequest = null;
               }

               state.bufferedRequestCount = 0;
               state.bufferedRequest = entry;
               state.bufferProcessing = false;
            }

            Writable.prototype._write = function (chunk, encoding, cb) {
               cb(new Error('_write() is not implemented'));
            };

            Writable.prototype._writev = null;

            Writable.prototype.end = function (chunk, encoding, cb) {
               var state = this._writableState;

               if (typeof chunk === 'function') {
                  cb = chunk;
                  chunk = null;
                  encoding = null;
               } else if (typeof encoding === 'function') {
                  cb = encoding;
                  encoding = null;
               }

               if (chunk !== null && chunk !== undefined) this.write(chunk, encoding);

               // .end() fully uncorks
               if (state.corked) {
                  state.corked = 1;
                  this.uncork();
               }

               // ignore unnecessary end() calls.
               if (!state.ending && !state.finished) endWritable(this, state, cb);
            };

            function needFinish(state) {
               return state.ending && state.length === 0 && state.bufferedRequest === null && !state.finished && !state.writing;
            }
            function callFinal(stream, state) {
               stream._final(function (err) {
                  state.pendingcb--;
                  if (err) {
                     stream.emit('error', err);
                  }
                  state.prefinished = true;
                  stream.emit('prefinish');
                  finishMaybe(stream, state);
               });
            }
            function prefinish(stream, state) {
               if (!state.prefinished && !state.finalCalled) {
                  if (typeof stream._final === 'function') {
                     state.pendingcb++;
                     state.finalCalled = true;
                     processNextTick(callFinal, stream, state);
                  } else {
                     state.prefinished = true;
                     stream.emit('prefinish');
                  }
               }
            }

            function finishMaybe(stream, state) {
               var need = needFinish(state);
               if (need) {
                  prefinish(stream, state);
                  if (state.pendingcb === 0) {
                     state.finished = true;
                     stream.emit('finish');
                  }
               }
               return need;
            }

            function endWritable(stream, state, cb) {
               state.ending = true;
               finishMaybe(stream, state);
               if (cb) {
                  if (state.finished) processNextTick(cb); else stream.once('finish', cb);
               }
               state.ended = true;
               stream.writable = false;
            }

            function onCorkedFinish(corkReq, state, err) {
               var entry = corkReq.entry;
               corkReq.entry = null;
               while (entry) {
                  var cb = entry.callback;
                  state.pendingcb--;
                  cb(err);
                  entry = entry.next;
               }
               if (state.corkedRequestsFree) {
                  state.corkedRequestsFree.next = corkReq;
               } else {
                  state.corkedRequestsFree = corkReq;
               }
            }

            Object.defineProperty(Writable.prototype, 'destroyed', {
               get: function get() {
                  if (this._writableState === undefined) {
                     return false;
                  }
                  return this._writableState.destroyed;
               },
               set: function set(value) {
                  // we ignore the value if the stream
                  // has not been initialized yet
                  if (!this._writableState) {
                     return;
                  }

                  // backward compatibility, the user is explicitly
                  // managing destroyed
                  this._writableState.destroyed = value;
               }
            });

            Writable.prototype.destroy = destroyImpl.destroy;
            Writable.prototype._undestroy = destroyImpl.undestroy;
            Writable.prototype._destroy = function (err, cb) {
               this.end();
               cb(err);
            };
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(6), __webpack_require__(46).setImmediate, __webpack_require__(1)))

         /***/
      }),
/* 25 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         /*<replacement>*/

         var processNextTick = __webpack_require__(10);
         /*</replacement>*/

         // undocumented cb() API, needed for core, not for public API
         function destroy(err, cb) {
            var _this = this;

            var readableDestroyed = this._readableState && this._readableState.destroyed;
            var writableDestroyed = this._writableState && this._writableState.destroyed;

            if (readableDestroyed || writableDestroyed) {
               if (cb) {
                  cb(err);
               } else if (err && (!this._writableState || !this._writableState.errorEmitted)) {
                  processNextTick(emitErrorNT, this, err);
               }
               return;
            }

            // we set destroyed to true before firing error callbacks in order
            // to make it re-entrance safe in case destroy() is called within callbacks

            if (this._readableState) {
               this._readableState.destroyed = true;
            }

            // if this is a duplex stream mark the writable part as destroyed as well
            if (this._writableState) {
               this._writableState.destroyed = true;
            }

            this._destroy(err || null, function (err) {
               if (!cb && err) {
                  processNextTick(emitErrorNT, _this, err);
                  if (_this._writableState) {
                     _this._writableState.errorEmitted = true;
                  }
               } else if (cb) {
                  cb(err);
               }
            });
         }

         function undestroy() {
            if (this._readableState) {
               this._readableState.destroyed = false;
               this._readableState.reading = false;
               this._readableState.ended = false;
               this._readableState.endEmitted = false;
            }

            if (this._writableState) {
               this._writableState.destroyed = false;
               this._writableState.ended = false;
               this._writableState.ending = false;
               this._writableState.finished = false;
               this._writableState.errorEmitted = false;
            }
         }

         function emitErrorNT(self, err) {
            self.emit('error', err);
         }

         module.exports = {
            destroy: destroy,
            undestroy: undestroy
         };

         /***/
      }),
/* 26 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         module.exports = __webpack_require__(19).EventEmitter;

         /***/
      }),
/* 27 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var regeneratorRuntime = __webpack_require__(15);
         var signalR = __webpack_require__(32);

         if (window) {
            window.regeneratorRuntime = window.regeneratorRuntime || regeneratorRuntime;
            window.signalR = window.signalR || signalR;
         }

         /***/
      }),
/* 28 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         Object.defineProperty(exports, "__esModule", { value: true });

         var Base64EncodedHubProtocol = function () {
            function Base64EncodedHubProtocol(protocol) {
               _classCallCheck(this, Base64EncodedHubProtocol);

               this.wrappedProtocol = protocol;
               this.name = this.wrappedProtocol.name;
               this.type = 1 /* Text */;
            }

            _createClass(Base64EncodedHubProtocol, [{
               key: "parseMessages",
               value: function parseMessages(input) {
                  // The format of the message is `size:message;`
                  var pos = input.indexOf(":");
                  if (pos == -1 || input[input.length - 1] != ';') {
                     throw new Error("Invalid payload.");
                  }
                  var lenStr = input.substring(0, pos);
                  if (!/^[0-9]+$/.test(lenStr)) {
                     throw new Error("Invalid length: '" + lenStr + "'");
                  }
                  var messageSize = parseInt(lenStr, 10);
                  // 2 accounts for ':' after message size and trailing ';'
                  if (messageSize != input.length - pos - 2) {
                     throw new Error("Invalid message size.");
                  }
                  var encodedMessage = input.substring(pos + 1, input.length - 1);
                  // atob/btoa are browsers APIs but they can be polyfilled. If this becomes problematic we can use
                  // base64-js module
                  var s = atob(encodedMessage);
                  var payload = new Uint8Array(s.length);
                  for (var i = 0; i < payload.length; i++) {
                     payload[i] = s.charCodeAt(i);
                  }
                  return this.wrappedProtocol.parseMessages(payload.buffer);
               }
            }, {
               key: "writeMessage",
               value: function writeMessage(message) {
                  var payload = new Uint8Array(this.wrappedProtocol.writeMessage(message));
                  var s = "";
                  for (var i = 0; i < payload.byteLength; i++) {
                     s += String.fromCharCode(payload[i]);
                  }
                  // atob/btoa are browsers APIs but they can be polyfilled. If this becomes problematic we can use
                  // base64-js module
                  var encodedMessage = btoa(s);
                  return encodedMessage.length.toString() + ":" + encodedMessage + ";";
               }
            }]);

            return Base64EncodedHubProtocol;
         }();

         exports.Base64EncodedHubProtocol = Base64EncodedHubProtocol;

         /***/
      }),
/* 29 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         var __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {
            return new (P || (P = Promise))(function (resolve, reject) {
               function fulfilled(value) {
                  try {
                     step(generator.next(value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function rejected(value) {
                  try {
                     step(generator["throw"](value));
                  } catch (e) {
                     reject(e);
                  }
               }
               function step(result) {
                  result.done ? resolve(result.value) : new P(function (resolve) {
                     resolve(result.value);
                  }).then(fulfilled, rejected);
               }
               step((generator = generator.apply(thisArg, _arguments || [])).next());
            });
         };
         Object.defineProperty(exports, "__esModule", { value: true });
         var HttpConnection_1 = __webpack_require__(12);
         var Observable_1 = __webpack_require__(31);
         var JsonHubProtocol_1 = __webpack_require__(18);
         var Formatters_1 = __webpack_require__(11);
         var Base64EncodedHubProtocol_1 = __webpack_require__(28);
         var ILogger_1 = __webpack_require__(3);
         var Loggers_1 = __webpack_require__(8);
         var Transports_1 = __webpack_require__(13);
         exports.TransportType = Transports_1.TransportType;
         var HttpConnection_2 = __webpack_require__(12);
         exports.HttpConnection = HttpConnection_2.HttpConnection;
         var JsonHubProtocol_2 = __webpack_require__(18);
         exports.JsonHubProtocol = JsonHubProtocol_2.JsonHubProtocol;
         var ILogger_2 = __webpack_require__(3);
         exports.LogLevel = ILogger_2.LogLevel;
         var Loggers_2 = __webpack_require__(8);
         exports.ConsoleLogger = Loggers_2.ConsoleLogger;
         exports.NullLogger = Loggers_2.NullLogger;

         var HubConnection = function () {
            function HubConnection(urlOrConnection) {
               var _this = this;

               var options = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};

               _classCallCheck(this, HubConnection);

               options = options || {};
               if (typeof urlOrConnection === "string") {
                  this.connection = new HttpConnection_1.HttpConnection(urlOrConnection, options);
               } else {
                  this.connection = urlOrConnection;
               }
               this.logger = Loggers_1.LoggerFactory.createLogger(options.logging);
               this.protocol = options.protocol || new JsonHubProtocol_1.JsonHubProtocol();
               this.connection.onreceive = function (data) {
                  return _this.processIncomingData(data);
               };
               this.connection.onclose = function (error) {
                  return _this.connectionClosed(error);
               };
               this.callbacks = new Map();
               this.methods = new Map();
               this.closedCallbacks = [];
               this.id = 0;
            }

            _createClass(HubConnection, [{
               key: "processIncomingData",
               value: function processIncomingData(data) {
                  // Parse the messages
                  var messages = this.protocol.parseMessages(data);
                  for (var i = 0; i < messages.length; ++i) {
                     var message = messages[i];
                     switch (message.type) {
                        case 1 /* Invocation */:
                           this.invokeClientMethod(message);
                           break;
                        case 2 /* Result */:
                        case 3 /* Completion */:
                           var callback = this.callbacks.get(message.invocationId);
                           if (callback != null) {
                              if (message.type == 3 /* Completion */) {
                                 this.callbacks.delete(message.invocationId);
                              }
                              callback(message);
                           }
                           break;
                        default:
                           this.logger.log(ILogger_1.LogLevel.Warning, "Invalid message type: " + data);
                           break;
                     }
                  }
               }
            }, {
               key: "invokeClientMethod",
               value: function invokeClientMethod(invocationMessage) {
                  var _this2 = this;

                  var methods = this.methods.get(invocationMessage.target.toLowerCase());
                  if (methods) {
                     methods.forEach(function (m) {
                        return m.apply(_this2, invocationMessage.arguments);
                     });
                     if (!invocationMessage.nonblocking) {
                        // TODO: send result back to the server?
                     }
                  } else {
                     this.logger.log(ILogger_1.LogLevel.Warning, "No client method with the name '" + invocationMessage.target + "' found.");
                  }
               }
            }, {
               key: "connectionClosed",
               value: function connectionClosed(error) {
                  var _this3 = this;

                  var errorCompletionMessage = {
                     type: 3 /* Completion */
                     , invocationId: "-1",
                     error: error ? error.message : "Invocation cancelled due to connection being closed."
                  };
                  this.callbacks.forEach(function (callback) {
                     callback(errorCompletionMessage);
                  });
                  this.callbacks.clear();
                  this.closedCallbacks.forEach(function (c) {
                     return c.apply(_this3, [error]);
                  });
               }
            }, {
               key: "start",
               value: function start() {
                  return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee() {
                     var requestedTransferMode, actualTransferMode;
                     return regeneratorRuntime.wrap(function _callee$(_context) {
                        while (1) {
                           switch (_context.prev = _context.next) {
                              case 0:
                                 requestedTransferMode = this.protocol.type === 2 /* Binary */ ? 2 /* Binary */
                                    : 1;

                                 this.connection.features.transferMode = requestedTransferMode;
                                 _context.next = 4;
                                 return this.connection.start();

                              case 4:
                                 actualTransferMode = this.connection.features.transferMode;
                                 _context.next = 7;
                                 return this.connection.send(Formatters_1.TextMessageFormat.write(JSON.stringify({ protocol: this.protocol.name })));

                              case 7:
                                 this.logger.log(ILogger_1.LogLevel.Information, "Using HubProtocol '" + this.protocol.name + "'.");
                                 if (requestedTransferMode === 2 /* Binary */ && actualTransferMode === 1 /* Text */) {
                                    this.protocol = new Base64EncodedHubProtocol_1.Base64EncodedHubProtocol(this.protocol);
                                 }

                              case 9:
                              case "end":
                                 return _context.stop();
                           }
                        }
                     }, _callee, this);
                  }));
               }
            }, {
               key: "stop",
               value: function stop() {
                  return this.connection.stop();
               }
            }, {
               key: "stream",
               value: function stream(methodName) {
                  var _this4 = this;

                  for (var _len = arguments.length, args = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
                     args[_key - 1] = arguments[_key];
                  }

                  var invocationDescriptor = this.createInvocation(methodName, args, false);
                  var subject = new Observable_1.Subject();
                  this.callbacks.set(invocationDescriptor.invocationId, function (invocationEvent) {
                     if (invocationEvent.type === 3 /* Completion */) {
                        var completionMessage = invocationEvent;
                        if (completionMessage.error) {
                           subject.error(new Error(completionMessage.error));
                        } else if (completionMessage.result) {
                           subject.error(new Error("Server provided a result in a completion response to a streamed invocation."));
                        } else {
                           // TODO: Log a warning if there's a payload?
                           subject.complete();
                        }
                     } else {
                        subject.next(invocationEvent.item);
                     }
                  });
                  var message = this.protocol.writeMessage(invocationDescriptor);
                  this.connection.send(message).catch(function (e) {
                     subject.error(e);
                     _this4.callbacks.delete(invocationDescriptor.invocationId);
                  });
                  return subject;
               }
            }, {
               key: "send",
               value: function send(methodName) {
                  for (var _len2 = arguments.length, args = Array(_len2 > 1 ? _len2 - 1 : 0), _key2 = 1; _key2 < _len2; _key2++) {
                     args[_key2 - 1] = arguments[_key2];
                  }

                  var invocationDescriptor = this.createInvocation(methodName, args, true);
                  var message = this.protocol.writeMessage(invocationDescriptor);
                  return this.connection.send(message);
               }
            }, {
               key: "invoke",
               value: function invoke(methodName) {
                  var _this5 = this;

                  for (var _len3 = arguments.length, args = Array(_len3 > 1 ? _len3 - 1 : 0), _key3 = 1; _key3 < _len3; _key3++) {
                     args[_key3 - 1] = arguments[_key3];
                  }

                  var invocationDescriptor = this.createInvocation(methodName, args, false);
                  var p = new Promise(function (resolve, reject) {
                     _this5.callbacks.set(invocationDescriptor.invocationId, function (invocationEvent) {
                        if (invocationEvent.type === 3 /* Completion */) {
                           var completionMessage = invocationEvent;
                           if (completionMessage.error) {
                              reject(new Error(completionMessage.error));
                           } else {
                              resolve(completionMessage.result);
                           }
                        } else {
                           reject(new Error("Streaming methods must be invoked using HubConnection.stream"));
                        }
                     });
                     var message = _this5.protocol.writeMessage(invocationDescriptor);
                     _this5.connection.send(message).catch(function (e) {
                        reject(e);
                        _this5.callbacks.delete(invocationDescriptor.invocationId);
                     });
                  });
                  return p;
               }
            }, {
               key: "on",
               value: function on(methodName, method) {
                  if (!methodName || !method) {
                     return;
                  }
                  methodName = methodName.toLowerCase();
                  if (!this.methods.has(methodName)) {
                     this.methods.set(methodName, []);
                  }
                  this.methods.get(methodName).push(method);
               }
            }, {
               key: "off",
               value: function off(methodName, method) {
                  if (!methodName || !method) {
                     return;
                  }
                  methodName = methodName.toLowerCase();
                  var handlers = this.methods.get(methodName);
                  if (!handlers) {
                     return;
                  }
                  var removeIdx = handlers.indexOf(method);
                  if (removeIdx != -1) {
                     handlers.splice(removeIdx, 1);
                  }
               }
            }, {
               key: "onclose",
               value: function onclose(callback) {
                  if (callback) {
                     this.closedCallbacks.push(callback);
                  }
               }
            }, {
               key: "createInvocation",
               value: function createInvocation(methodName, args, nonblocking) {
                  var id = this.id;
                  this.id++;
                  return {
                     type: 1 /* Invocation */
                     , invocationId: id.toString(),
                     target: methodName,
                     arguments: args,
                     nonblocking: nonblocking
                  };
               }
            }]);

            return HubConnection;
         }();

         exports.HubConnection = HubConnection;

         /***/
      }),
/* 30 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (Buffer) {
            // Copyright (c) .NET Foundation. All rights reserved.
            // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

            var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

            function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

            Object.defineProperty(exports, "__esModule", { value: true });
            var Formatters_1 = __webpack_require__(11);
            var msgpack5 = __webpack_require__(36);

            var MessagePackHubProtocol = function () {
               function MessagePackHubProtocol() {
                  _classCallCheck(this, MessagePackHubProtocol);

                  this.name = "messagepack";
                  this.type = 2 /* Binary */;
               }

               _createClass(MessagePackHubProtocol, [{
                  key: "parseMessages",
                  value: function parseMessages(input) {
                     var _this = this;

                     return Formatters_1.BinaryMessageFormat.parse(input).map(function (m) {
                        return _this.parseMessage(m);
                     });
                  }
               }, {
                  key: "parseMessage",
                  value: function parseMessage(input) {
                     if (input.length == 0) {
                        throw new Error("Invalid payload.");
                     }
                     var msgpack = msgpack5();
                     var properties = msgpack.decode(new Buffer(input));
                     if (properties.length == 0 || !(properties instanceof Array)) {
                        throw new Error("Invalid payload.");
                     }
                     var messageType = properties[0];
                     switch (messageType) {
                        case 1 /* Invocation */:
                           return this.createInvocationMessage(properties);
                        case 2 /* Result */:
                           return this.createStreamItemMessage(properties);
                        case 3 /* Completion */:
                           return this.createCompletionMessage(properties);
                        default:
                           throw new Error("Invalid message type.");
                     }
                  }
               }, {
                  key: "createInvocationMessage",
                  value: function createInvocationMessage(properties) {
                     if (properties.length != 5) {
                        throw new Error("Invalid payload for Invocation message.");
                     }
                     return {
                        type: 1 /* Invocation */
                        , invocationId: properties[1],
                        nonblocking: properties[2],
                        target: properties[3],
                        arguments: properties[4]
                     };
                  }
               }, {
                  key: "createStreamItemMessage",
                  value: function createStreamItemMessage(properties) {
                     if (properties.length != 3) {
                        throw new Error("Invalid payload for stream Result message.");
                     }
                     return {
                        type: 2 /* Result */
                        , invocationId: properties[1],
                        item: properties[2]
                     };
                  }
               }, {
                  key: "createCompletionMessage",
                  value: function createCompletionMessage(properties) {
                     if (properties.length < 3) {
                        throw new Error("Invalid payload for Completion message.");
                     }
                     var errorResult = 1;
                     var voidResult = 2;
                     var nonVoidResult = 3;
                     var resultKind = properties[2];
                     if (resultKind === voidResult && properties.length != 3 || resultKind !== voidResult && properties.length != 4) {
                        throw new Error("Invalid payload for Completion message.");
                     }
                     var completionMessage = {
                        type: 3 /* Completion */
                        , invocationId: properties[1],
                        error: null,
                        result: null
                     };
                     switch (resultKind) {
                        case errorResult:
                           completionMessage.error = properties[3];
                           break;
                        case nonVoidResult:
                           completionMessage.result = properties[3];
                           break;
                     }
                     return completionMessage;
                  }
               }, {
                  key: "writeMessage",
                  value: function writeMessage(message) {
                     switch (message.type) {
                        case 1 /* Invocation */:
                           return this.writeInvocation(message);
                        case 2 /* Result */:
                        case 3 /* Completion */:
                           throw new Error("Writing messages of type '" + message.type + "' is not supported.");
                        default:
                           throw new Error("Invalid message type.");
                     }
                  }
               }, {
                  key: "writeInvocation",
                  value: function writeInvocation(invocationMessage) {
                     var msgpack = msgpack5();
                     var payload = msgpack.encode([1 /* Invocation */, invocationMessage.invocationId, invocationMessage.nonblocking, invocationMessage.target, invocationMessage.arguments]);
                     return Formatters_1.BinaryMessageFormat.write(payload.slice());
                  }
               }]);

               return MessagePackHubProtocol;
            }();

            exports.MessagePackHubProtocol = MessagePackHubProtocol;
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(4).Buffer))

         /***/
      }),
/* 31 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

         function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

         Object.defineProperty(exports, "__esModule", { value: true });

         var Subject = function () {
            function Subject() {
               _classCallCheck(this, Subject);

               this.observers = [];
            }

            _createClass(Subject, [{
               key: "next",
               value: function next(item) {
                  var _iteratorNormalCompletion = true;
                  var _didIteratorError = false;
                  var _iteratorError = undefined;

                  try {
                     for (var _iterator = this.observers[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
                        var observer = _step.value;

                        observer.next(item);
                     }
                  } catch (err) {
                     _didIteratorError = true;
                     _iteratorError = err;
                  } finally {
                     try {
                        if (!_iteratorNormalCompletion && _iterator.return) {
                           _iterator.return();
                        }
                     } finally {
                        if (_didIteratorError) {
                           throw _iteratorError;
                        }
                     }
                  }
               }
            }, {
               key: "error",
               value: function error(err) {
                  var _iteratorNormalCompletion2 = true;
                  var _didIteratorError2 = false;
                  var _iteratorError2 = undefined;

                  try {
                     for (var _iterator2 = this.observers[Symbol.iterator](), _step2; !(_iteratorNormalCompletion2 = (_step2 = _iterator2.next()).done); _iteratorNormalCompletion2 = true) {
                        var observer = _step2.value;

                        observer.error(err);
                     }
                  } catch (err) {
                     _didIteratorError2 = true;
                     _iteratorError2 = err;
                  } finally {
                     try {
                        if (!_iteratorNormalCompletion2 && _iterator2.return) {
                           _iterator2.return();
                        }
                     } finally {
                        if (_didIteratorError2) {
                           throw _iteratorError2;
                        }
                     }
                  }
               }
            }, {
               key: "complete",
               value: function complete() {
                  var _iteratorNormalCompletion3 = true;
                  var _didIteratorError3 = false;
                  var _iteratorError3 = undefined;

                  try {
                     for (var _iterator3 = this.observers[Symbol.iterator](), _step3; !(_iteratorNormalCompletion3 = (_step3 = _iterator3.next()).done); _iteratorNormalCompletion3 = true) {
                        var observer = _step3.value;

                        observer.complete();
                     }
                  } catch (err) {
                     _didIteratorError3 = true;
                     _iteratorError3 = err;
                  } finally {
                     try {
                        if (!_iteratorNormalCompletion3 && _iterator3.return) {
                           _iterator3.return();
                        }
                     } finally {
                        if (_didIteratorError3) {
                           throw _iteratorError3;
                        }
                     }
                  }
               }
            }, {
               key: "subscribe",
               value: function subscribe(observer) {
                  this.observers.push(observer);
               }
            }]);

            return Subject;
         }();

         exports.Subject = Subject;

         /***/
      }),
/* 32 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";

         // Copyright (c) .NET Foundation. All rights reserved.
         // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

         function __export(m) {
            for (var p in m) {
               if (!exports.hasOwnProperty(p)) exports[p] = m[p];
            }
         }
         Object.defineProperty(exports, "__esModule", { value: true });
         __export(__webpack_require__(12));
         __export(__webpack_require__(16));
         __export(__webpack_require__(29));
         __export(__webpack_require__(13));
         __export(__webpack_require__(8));
         __export(__webpack_require__(30));

         /***/
      }),
/* 33 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global) {

            // compare and isBuffer taken from https://github.com/feross/buffer/blob/680e9e5e488f22aac27599a57dc844a6315928dd/index.js
            // original notice:

            /*!
             * The buffer module from node.js, for the browser.
             *
             * @author   Feross Aboukhadijeh <feross@feross.org> <http://feross.org>
             * @license  MIT
             */

            var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

            function compare(a, b) {
               if (a === b) {
                  return 0;
               }

               var x = a.length;
               var y = b.length;

               for (var i = 0, len = Math.min(x, y); i < len; ++i) {
                  if (a[i] !== b[i]) {
                     x = a[i];
                     y = b[i];
                     break;
                  }
               }

               if (x < y) {
                  return -1;
               }
               if (y < x) {
                  return 1;
               }
               return 0;
            }
            function isBuffer(b) {
               if (global.Buffer && typeof global.Buffer.isBuffer === 'function') {
                  return global.Buffer.isBuffer(b);
               }
               return !!(b != null && b._isBuffer);
            }

            // based on node assert, original notice:

            // http://wiki.commonjs.org/wiki/Unit_Testing/1.0
            //
            // THIS IS NOT TESTED NOR LIKELY TO WORK OUTSIDE V8!
            //
            // Originally from narwhal.js (http://narwhaljs.org)
            // Copyright (c) 2009 Thomas Robinson <280north.com>
            //
            // Permission is hereby granted, free of charge, to any person obtaining a copy
            // of this software and associated documentation files (the 'Software'), to
            // deal in the Software without restriction, including without limitation the
            // rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
            // sell copies of the Software, and to permit persons to whom the Software is
            // furnished to do so, subject to the following conditions:
            //
            // The above copyright notice and this permission notice shall be included in
            // all copies or substantial portions of the Software.
            //
            // THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
            // AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
            // ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
            // WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

            var util = __webpack_require__(14);
            var hasOwn = Object.prototype.hasOwnProperty;
            var pSlice = Array.prototype.slice;
            var functionsHaveNames = function () {
               return function foo() { }.name === 'foo';
            }();
            function pToString(obj) {
               return Object.prototype.toString.call(obj);
            }
            function isView(arrbuf) {
               if (isBuffer(arrbuf)) {
                  return false;
               }
               if (typeof global.ArrayBuffer !== 'function') {
                  return false;
               }
               if (typeof ArrayBuffer.isView === 'function') {
                  return ArrayBuffer.isView(arrbuf);
               }
               if (!arrbuf) {
                  return false;
               }
               if (arrbuf instanceof DataView) {
                  return true;
               }
               if (arrbuf.buffer && arrbuf.buffer instanceof ArrayBuffer) {
                  return true;
               }
               return false;
            }
            // 1. The assert module provides functions that throw
            // AssertionError's when particular conditions are not met. The
            // assert module must conform to the following interface.

            var assert = module.exports = ok;

            // 2. The AssertionError is defined in assert.
            // new assert.AssertionError({ message: message,
            //                             actual: actual,
            //                             expected: expected })

            var regex = /\s*function\s+([^\(\s]*)\s*/;
            // based on https://github.com/ljharb/function.prototype.name/blob/adeeeec8bfcc6068b187d7d9fb3d5bb1d3a30899/implementation.js
            function getName(func) {
               if (!util.isFunction(func)) {
                  return;
               }
               if (functionsHaveNames) {
                  return func.name;
               }
               var str = func.toString();
               var match = str.match(regex);
               return match && match[1];
            }
            assert.AssertionError = function AssertionError(options) {
               this.name = 'AssertionError';
               this.actual = options.actual;
               this.expected = options.expected;
               this.operator = options.operator;
               if (options.message) {
                  this.message = options.message;
                  this.generatedMessage = false;
               } else {
                  this.message = getMessage(this);
                  this.generatedMessage = true;
               }
               var stackStartFunction = options.stackStartFunction || fail;
               if (Error.captureStackTrace) {
                  Error.captureStackTrace(this, stackStartFunction);
               } else {
                  // non v8 browsers so we can have a stacktrace
                  var err = new Error();
                  if (err.stack) {
                     var out = err.stack;

                     // try to strip useless frames
                     var fn_name = getName(stackStartFunction);
                     var idx = out.indexOf('\n' + fn_name);
                     if (idx >= 0) {
                        // once we have located the function frame
                        // we need to strip out everything before it (and its line)
                        var next_line = out.indexOf('\n', idx + 1);
                        out = out.substring(next_line + 1);
                     }

                     this.stack = out;
                  }
               }
            };

            // assert.AssertionError instanceof Error
            util.inherits(assert.AssertionError, Error);

            function truncate(s, n) {
               if (typeof s === 'string') {
                  return s.length < n ? s : s.slice(0, n);
               } else {
                  return s;
               }
            }
            function inspect(something) {
               if (functionsHaveNames || !util.isFunction(something)) {
                  return util.inspect(something);
               }
               var rawname = getName(something);
               var name = rawname ? ': ' + rawname : '';
               return '[Function' + name + ']';
            }
            function getMessage(self) {
               return truncate(inspect(self.actual), 128) + ' ' + self.operator + ' ' + truncate(inspect(self.expected), 128);
            }

            // At present only the three keys mentioned above are used and
            // understood by the spec. Implementations or sub modules can pass
            // other keys to the AssertionError's constructor - they will be
            // ignored.

            // 3. All of the following functions must throw an AssertionError
            // when a corresponding condition is not met, with a message that
            // may be undefined if not provided.  All assertion methods provide
            // both the actual and expected values to the assertion error for
            // display purposes.

            function fail(actual, expected, message, operator, stackStartFunction) {
               throw new assert.AssertionError({
                  message: message,
                  actual: actual,
                  expected: expected,
                  operator: operator,
                  stackStartFunction: stackStartFunction
               });
            }

            // EXTENSION! allows for well behaved errors defined elsewhere.
            assert.fail = fail;

            // 4. Pure assertion tests whether a value is truthy, as determined
            // by !!guard.
            // assert.ok(guard, message_opt);
            // This statement is equivalent to assert.equal(true, !!guard,
            // message_opt);. To test strictly for the value true, use
            // assert.strictEqual(true, guard, message_opt);.

            function ok(value, message) {
               if (!value) fail(value, true, message, '==', assert.ok);
            }
            assert.ok = ok;

            // 5. The equality assertion tests shallow, coercive equality with
            // ==.
            // assert.equal(actual, expected, message_opt);

            assert.equal = function equal(actual, expected, message) {
               if (actual != expected) fail(actual, expected, message, '==', assert.equal);
            };

            // 6. The non-equality assertion tests for whether two objects are not equal
            // with != assert.notEqual(actual, expected, message_opt);

            assert.notEqual = function notEqual(actual, expected, message) {
               if (actual == expected) {
                  fail(actual, expected, message, '!=', assert.notEqual);
               }
            };

            // 7. The equivalence assertion tests a deep equality relation.
            // assert.deepEqual(actual, expected, message_opt);

            assert.deepEqual = function deepEqual(actual, expected, message) {
               if (!_deepEqual(actual, expected, false)) {
                  fail(actual, expected, message, 'deepEqual', assert.deepEqual);
               }
            };

            assert.deepStrictEqual = function deepStrictEqual(actual, expected, message) {
               if (!_deepEqual(actual, expected, true)) {
                  fail(actual, expected, message, 'deepStrictEqual', assert.deepStrictEqual);
               }
            };

            function _deepEqual(actual, expected, strict, memos) {
               // 7.1. All identical values are equivalent, as determined by ===.
               if (actual === expected) {
                  return true;
               } else if (isBuffer(actual) && isBuffer(expected)) {
                  return compare(actual, expected) === 0;

                  // 7.2. If the expected value is a Date object, the actual value is
                  // equivalent if it is also a Date object that refers to the same time.
               } else if (util.isDate(actual) && util.isDate(expected)) {
                  return actual.getTime() === expected.getTime();

                  // 7.3 If the expected value is a RegExp object, the actual value is
                  // equivalent if it is also a RegExp object with the same source and
                  // properties (`global`, `multiline`, `lastIndex`, `ignoreCase`).
               } else if (util.isRegExp(actual) && util.isRegExp(expected)) {
                  return actual.source === expected.source && actual.global === expected.global && actual.multiline === expected.multiline && actual.lastIndex === expected.lastIndex && actual.ignoreCase === expected.ignoreCase;

                  // 7.4. Other pairs that do not both pass typeof value == 'object',
                  // equivalence is determined by ==.
               } else if ((actual === null || (typeof actual === 'undefined' ? 'undefined' : _typeof(actual)) !== 'object') && (expected === null || (typeof expected === 'undefined' ? 'undefined' : _typeof(expected)) !== 'object')) {
                  return strict ? actual === expected : actual == expected;

                  // If both values are instances of typed arrays, wrap their underlying
                  // ArrayBuffers in a Buffer each to increase performance
                  // This optimization requires the arrays to have the same type as checked by
                  // Object.prototype.toString (aka pToString). Never perform binary
                  // comparisons for Float*Arrays, though, since e.g. +0 === -0 but their
                  // bit patterns are not identical.
               } else if (isView(actual) && isView(expected) && pToString(actual) === pToString(expected) && !(actual instanceof Float32Array || actual instanceof Float64Array)) {
                  return compare(new Uint8Array(actual.buffer), new Uint8Array(expected.buffer)) === 0;

                  // 7.5 For all other Object pairs, including Array objects, equivalence is
                  // determined by having the same number of owned properties (as verified
                  // with Object.prototype.hasOwnProperty.call), the same set of keys
                  // (although not necessarily the same order), equivalent values for every
                  // corresponding key, and an identical 'prototype' property. Note: this
                  // accounts for both named and indexed properties on Arrays.
               } else if (isBuffer(actual) !== isBuffer(expected)) {
                  return false;
               } else {
                  memos = memos || { actual: [], expected: [] };

                  var actualIndex = memos.actual.indexOf(actual);
                  if (actualIndex !== -1) {
                     if (actualIndex === memos.expected.indexOf(expected)) {
                        return true;
                     }
                  }

                  memos.actual.push(actual);
                  memos.expected.push(expected);

                  return objEquiv(actual, expected, strict, memos);
               }
            }

            function isArguments(object) {
               return Object.prototype.toString.call(object) == '[object Arguments]';
            }

            function objEquiv(a, b, strict, actualVisitedObjects) {
               if (a === null || a === undefined || b === null || b === undefined) return false;
               // if one is a primitive, the other must be same
               if (util.isPrimitive(a) || util.isPrimitive(b)) return a === b;
               if (strict && Object.getPrototypeOf(a) !== Object.getPrototypeOf(b)) return false;
               var aIsArgs = isArguments(a);
               var bIsArgs = isArguments(b);
               if (aIsArgs && !bIsArgs || !aIsArgs && bIsArgs) return false;
               if (aIsArgs) {
                  a = pSlice.call(a);
                  b = pSlice.call(b);
                  return _deepEqual(a, b, strict);
               }
               var ka = objectKeys(a);
               var kb = objectKeys(b);
               var key, i;
               // having the same number of owned properties (keys incorporates
               // hasOwnProperty)
               if (ka.length !== kb.length) return false;
               //the same set of keys (although not necessarily the same order),
               ka.sort();
               kb.sort();
               //~~~cheap key test
               for (i = ka.length - 1; i >= 0; i--) {
                  if (ka[i] !== kb[i]) return false;
               }
               //equivalent values for every corresponding key, and
               //~~~possibly expensive deep test
               for (i = ka.length - 1; i >= 0; i--) {
                  key = ka[i];
                  if (!_deepEqual(a[key], b[key], strict, actualVisitedObjects)) return false;
               }
               return true;
            }

            // 8. The non-equivalence assertion tests for any deep inequality.
            // assert.notDeepEqual(actual, expected, message_opt);

            assert.notDeepEqual = function notDeepEqual(actual, expected, message) {
               if (_deepEqual(actual, expected, false)) {
                  fail(actual, expected, message, 'notDeepEqual', assert.notDeepEqual);
               }
            };

            assert.notDeepStrictEqual = notDeepStrictEqual;
            function notDeepStrictEqual(actual, expected, message) {
               if (_deepEqual(actual, expected, true)) {
                  fail(actual, expected, message, 'notDeepStrictEqual', notDeepStrictEqual);
               }
            }

            // 9. The strict equality assertion tests strict equality, as determined by ===.
            // assert.strictEqual(actual, expected, message_opt);

            assert.strictEqual = function strictEqual(actual, expected, message) {
               if (actual !== expected) {
                  fail(actual, expected, message, '===', assert.strictEqual);
               }
            };

            // 10. The strict non-equality assertion tests for strict inequality, as
            // determined by !==.  assert.notStrictEqual(actual, expected, message_opt);

            assert.notStrictEqual = function notStrictEqual(actual, expected, message) {
               if (actual === expected) {
                  fail(actual, expected, message, '!==', assert.notStrictEqual);
               }
            };

            function expectedException(actual, expected) {
               if (!actual || !expected) {
                  return false;
               }

               if (Object.prototype.toString.call(expected) == '[object RegExp]') {
                  return expected.test(actual);
               }

               try {
                  if (actual instanceof expected) {
                     return true;
                  }
               } catch (e) {
                  // Ignore.  The instanceof check doesn't work for arrow functions.
               }

               if (Error.isPrototypeOf(expected)) {
                  return false;
               }

               return expected.call({}, actual) === true;
            }

            function _tryBlock(block) {
               var error;
               try {
                  block();
               } catch (e) {
                  error = e;
               }
               return error;
            }

            function _throws(shouldThrow, block, expected, message) {
               var actual;

               if (typeof block !== 'function') {
                  throw new TypeError('"block" argument must be a function');
               }

               if (typeof expected === 'string') {
                  message = expected;
                  expected = null;
               }

               actual = _tryBlock(block);

               message = (expected && expected.name ? ' (' + expected.name + ').' : '.') + (message ? ' ' + message : '.');

               if (shouldThrow && !actual) {
                  fail(actual, expected, 'Missing expected exception' + message);
               }

               var userProvidedMessage = typeof message === 'string';
               var isUnwantedException = !shouldThrow && util.isError(actual);
               var isUnexpectedException = !shouldThrow && actual && !expected;

               if (isUnwantedException && userProvidedMessage && expectedException(actual, expected) || isUnexpectedException) {
                  fail(actual, expected, 'Got unwanted exception' + message);
               }

               if (shouldThrow && actual && expected && !expectedException(actual, expected) || !shouldThrow && actual) {
                  throw actual;
               }
            }

            // 11. Expected to throw an error:
            // assert.throws(block, Error_opt, message_opt);

            assert.throws = function (block, /*optional*/error, /*optional*/message) {
               _throws(true, block, error, message);
            };

            // EXTENSION! This is annoying to write outside this module.
            assert.doesNotThrow = function (block, /*optional*/error, /*optional*/message) {
               _throws(false, block, error, message);
            };

            assert.ifError = function (err) {
               if (err) throw err;
            };

            var objectKeys = Object.keys || function (obj) {
               var keys = [];
               for (var key in obj) {
                  if (hasOwn.call(obj, key)) keys.push(key);
               }
               return keys;
            };
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1)))

         /***/
      }),
/* 34 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         exports.byteLength = byteLength;
         exports.toByteArray = toByteArray;
         exports.fromByteArray = fromByteArray;

         var lookup = [];
         var revLookup = [];
         var Arr = typeof Uint8Array !== 'undefined' ? Uint8Array : Array;

         var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
         for (var i = 0, len = code.length; i < len; ++i) {
            lookup[i] = code[i];
            revLookup[code.charCodeAt(i)] = i;
         }

         revLookup['-'.charCodeAt(0)] = 62;
         revLookup['_'.charCodeAt(0)] = 63;

         function placeHoldersCount(b64) {
            var len = b64.length;
            if (len % 4 > 0) {
               throw new Error('Invalid string. Length must be a multiple of 4');
            }

            // the number of equal signs (place holders)
            // if there are two placeholders, than the two characters before it
            // represent one byte
            // if there is only one, then the three characters before it represent 2 bytes
            // this is just a cheap hack to not do indexOf twice
            return b64[len - 2] === '=' ? 2 : b64[len - 1] === '=' ? 1 : 0;
         }

         function byteLength(b64) {
            // base64 is 4/3 + up to two characters of the original data
            return b64.length * 3 / 4 - placeHoldersCount(b64);
         }

         function toByteArray(b64) {
            var i, l, tmp, placeHolders, arr;
            var len = b64.length;
            placeHolders = placeHoldersCount(b64);

            arr = new Arr(len * 3 / 4 - placeHolders);

            // if there are placeholders, only get up to the last complete 4 chars
            l = placeHolders > 0 ? len - 4 : len;

            var L = 0;

            for (i = 0; i < l; i += 4) {
               tmp = revLookup[b64.charCodeAt(i)] << 18 | revLookup[b64.charCodeAt(i + 1)] << 12 | revLookup[b64.charCodeAt(i + 2)] << 6 | revLookup[b64.charCodeAt(i + 3)];
               arr[L++] = tmp >> 16 & 0xFF;
               arr[L++] = tmp >> 8 & 0xFF;
               arr[L++] = tmp & 0xFF;
            }

            if (placeHolders === 2) {
               tmp = revLookup[b64.charCodeAt(i)] << 2 | revLookup[b64.charCodeAt(i + 1)] >> 4;
               arr[L++] = tmp & 0xFF;
            } else if (placeHolders === 1) {
               tmp = revLookup[b64.charCodeAt(i)] << 10 | revLookup[b64.charCodeAt(i + 1)] << 4 | revLookup[b64.charCodeAt(i + 2)] >> 2;
               arr[L++] = tmp >> 8 & 0xFF;
               arr[L++] = tmp & 0xFF;
            }

            return arr;
         }

         function tripletToBase64(num) {
            return lookup[num >> 18 & 0x3F] + lookup[num >> 12 & 0x3F] + lookup[num >> 6 & 0x3F] + lookup[num & 0x3F];
         }

         function encodeChunk(uint8, start, end) {
            var tmp;
            var output = [];
            for (var i = start; i < end; i += 3) {
               tmp = (uint8[i] << 16) + (uint8[i + 1] << 8) + uint8[i + 2];
               output.push(tripletToBase64(tmp));
            }
            return output.join('');
         }

         function fromByteArray(uint8) {
            var tmp;
            var len = uint8.length;
            var extraBytes = len % 3; // if we have 1 byte left, pad 2 bytes
            var output = '';
            var parts = [];
            var maxChunkLength = 16383; // must be multiple of 3

            // go through the array every three bytes, we'll deal with trailing stuff later
            for (var i = 0, len2 = len - extraBytes; i < len2; i += maxChunkLength) {
               parts.push(encodeChunk(uint8, i, i + maxChunkLength > len2 ? len2 : i + maxChunkLength));
            }

            // pad the end with zeros, but make sure to not forget the extra bytes
            if (extraBytes === 1) {
               tmp = uint8[len - 1];
               output += lookup[tmp >> 2];
               output += lookup[tmp << 4 & 0x3F];
               output += '==';
            } else if (extraBytes === 2) {
               tmp = (uint8[len - 2] << 8) + uint8[len - 1];
               output += lookup[tmp >> 10];
               output += lookup[tmp >> 4 & 0x3F];
               output += lookup[tmp << 2 & 0x3F];
               output += '=';
            }

            parts.push(output);

            return parts.join('');
         }

         /***/
      }),
/* 35 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         exports.read = function (buffer, offset, isLE, mLen, nBytes) {
            var e, m;
            var eLen = nBytes * 8 - mLen - 1;
            var eMax = (1 << eLen) - 1;
            var eBias = eMax >> 1;
            var nBits = -7;
            var i = isLE ? nBytes - 1 : 0;
            var d = isLE ? -1 : 1;
            var s = buffer[offset + i];

            i += d;

            e = s & (1 << -nBits) - 1;
            s >>= -nBits;
            nBits += eLen;
            for (; nBits > 0; e = e * 256 + buffer[offset + i], i += d, nBits -= 8) { }

            m = e & (1 << -nBits) - 1;
            e >>= -nBits;
            nBits += mLen;
            for (; nBits > 0; m = m * 256 + buffer[offset + i], i += d, nBits -= 8) { }

            if (e === 0) {
               e = 1 - eBias;
            } else if (e === eMax) {
               return m ? NaN : (s ? -1 : 1) * Infinity;
            } else {
               m = m + Math.pow(2, mLen);
               e = e - eBias;
            }
            return (s ? -1 : 1) * m * Math.pow(2, e - mLen);
         };

         exports.write = function (buffer, value, offset, isLE, mLen, nBytes) {
            var e, m, c;
            var eLen = nBytes * 8 - mLen - 1;
            var eMax = (1 << eLen) - 1;
            var eBias = eMax >> 1;
            var rt = mLen === 23 ? Math.pow(2, -24) - Math.pow(2, -77) : 0;
            var i = isLE ? 0 : nBytes - 1;
            var d = isLE ? 1 : -1;
            var s = value < 0 || value === 0 && 1 / value < 0 ? 1 : 0;

            value = Math.abs(value);

            if (isNaN(value) || value === Infinity) {
               m = isNaN(value) ? 1 : 0;
               e = eMax;
            } else {
               e = Math.floor(Math.log(value) / Math.LN2);
               if (value * (c = Math.pow(2, -e)) < 1) {
                  e--;
                  c *= 2;
               }
               if (e + eBias >= 1) {
                  value += rt / c;
               } else {
                  value += rt * Math.pow(2, 1 - eBias);
               }
               if (value * c >= 2) {
                  e++;
                  c /= 2;
               }

               if (e + eBias >= eMax) {
                  m = 0;
                  e = eMax;
               } else if (e + eBias >= 1) {
                  m = (value * c - 1) * Math.pow(2, mLen);
                  e = e + eBias;
               } else {
                  m = value * Math.pow(2, eBias - 1) * Math.pow(2, mLen);
                  e = 0;
               }
            }

            for (; mLen >= 8; buffer[offset + i] = m & 0xff, i += d, m /= 256, mLen -= 8) { }

            e = e << mLen | m;
            eLen += mLen;
            for (; eLen > 0; buffer[offset + i] = e & 0xff, i += d, e /= 256, eLen -= 8) { }

            buffer[offset + i - d] |= s * 128;
         };

         /***/
      }),
/* 36 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var Buffer = __webpack_require__(7).Buffer;
         var assert = __webpack_require__(33);
         var bl = __webpack_require__(9);
         var streams = __webpack_require__(39);
         var buildDecode = __webpack_require__(37);
         var buildEncode = __webpack_require__(38);

         function msgpack(options) {
            var encodingTypes = [];
            var decodingTypes = [];

            options = options || {
               forceFloat64: false,
               compatibilityMode: false
            };

            function registerEncoder(check, encode) {
               assert(check, 'must have an encode function');
               assert(encode, 'must have an encode function');

               encodingTypes.push({
                  check: check, encode: encode
               });

               return this;
            }

            function registerDecoder(type, decode) {
               assert(type >= 0, 'must have a non-negative type');
               assert(decode, 'must have a decode function');

               decodingTypes.push({
                  type: type, decode: decode
               });

               return this;
            }

            function register(type, constructor, encode, decode) {
               assert(constructor, 'must have a constructor');
               assert(encode, 'must have an encode function');
               assert(type >= 0, 'must have a non-negative type');
               assert(decode, 'must have a decode function');

               function check(obj) {
                  return obj instanceof constructor;
               }

               function reEncode(obj) {
                  var buf = bl();
                  var header = Buffer.allocUnsafe(1);

                  header.writeInt8(type, 0);

                  buf.append(header);
                  buf.append(encode(obj));

                  return buf;
               }

               this.registerEncoder(check, reEncode);
               this.registerDecoder(type, decode);

               return this;
            }

            return {
               encode: buildEncode(encodingTypes, options.forceFloat64, options.compatibilityMode),
               decode: buildDecode(decodingTypes),
               register: register,
               registerEncoder: registerEncoder,
               registerDecoder: registerDecoder,
               encoder: streams.encoder,
               decoder: streams.decoder,
               // needed for levelup support
               buffer: true,
               type: 'msgpack5',
               IncompleteBufferError: buildDecode.IncompleteBufferError
            };
         }

         module.exports = msgpack;

         /***/
      }),
/* 37 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var bl = __webpack_require__(9);
         var util = __webpack_require__(14);

         function IncompleteBufferError(message) {
            Error.call(this); // super constructor
            if (Error.captureStackTrace) {
               Error.captureStackTrace(this, this.constructor); // super helper method to include stack trace in error object
            }
            this.name = this.constructor.name;
            this.message = message || 'unable to decode';
         }

         util.inherits(IncompleteBufferError, Error);

         module.exports = function buildDecode(decodingTypes) {
            return decode;

            function getSize(first) {
               switch (first) {
                  case 0xc4:
                     return 2;
                  case 0xc5:
                     return 3;
                  case 0xc6:
                     return 5;
                  case 0xc7:
                     return 3;
                  case 0xc8:
                     return 4;
                  case 0xc9:
                     return 6;
                  case 0xca:
                     return 5;
                  case 0xcb:
                     return 9;
                  case 0xcc:
                     return 2;
                  case 0xcd:
                     return 3;
                  case 0xce:
                     return 5;
                  case 0xcf:
                     return 9;
                  case 0xd0:
                     return 2;
                  case 0xd1:
                     return 3;
                  case 0xd2:
                     return 5;
                  case 0xd3:
                     return 9;
                  case 0xd4:
                     return 3;
                  case 0xd5:
                     return 4;
                  case 0xd6:
                     return 6;
                  case 0xd7:
                     return 10;
                  case 0xd8:
                     return 18;
                  case 0xd9:
                     return 2;
                  case 0xda:
                     return 3;
                  case 0xdb:
                     return 5;
                  case 0xde:
                     return 3;
                  default:
                     return -1;
               }
            }

            function hasMinBufferSize(first, length) {
               var size = getSize(first);

               if (size !== -1 && length < size) {
                  return false;
               } else {
                  return true;
               }
            }

            function isValidDataSize(dataLength, bufLength, headerLength) {
               return bufLength >= headerLength + dataLength;
            }

            function buildDecodeResult(value, bytesConsumed) {
               return {
                  value: value,
                  bytesConsumed: bytesConsumed
               };
            }

            function decode(buf) {
               if (!(buf instanceof bl)) {
                  buf = bl().append(buf);
               }

               var result = tryDecode(buf);
               if (result) {
                  buf.consume(result.bytesConsumed);
                  return result.value;
               } else {
                  throw new IncompleteBufferError();
               }
            }

            function tryDecode(buf, offset) {
               offset = offset === undefined ? 0 : offset;
               var bufLength = buf.length - offset;
               if (bufLength <= 0) {
                  return null;
               }

               var first = buf.readUInt8(offset);
               var length;
               var result = 0;
               var type;
               var bytePos;

               if (!hasMinBufferSize(first, bufLength)) {
                  return null;
               }

               switch (first) {
                  case 0xc0:
                     return buildDecodeResult(null, 1);
                  case 0xc2:
                     return buildDecodeResult(false, 1);
                  case 0xc3:
                     return buildDecodeResult(true, 1);
                  case 0xcc:
                     // 1-byte unsigned int
                     result = buf.readUInt8(offset + 1);
                     return buildDecodeResult(result, 2);
                  case 0xcd:
                     // 2-bytes BE unsigned int
                     result = buf.readUInt16BE(offset + 1);
                     return buildDecodeResult(result, 3);
                  case 0xce:
                     // 4-bytes BE unsigned int
                     result = buf.readUInt32BE(offset + 1);
                     return buildDecodeResult(result, 5);
                  case 0xcf:
                     // 8-bytes BE unsigned int
                     // Read long byte by byte, big-endian
                     for (bytePos = 7; bytePos >= 0; bytePos--) {
                        result += buf.readUInt8(offset + bytePos + 1) * Math.pow(2, 8 * (7 - bytePos));
                     }
                     return buildDecodeResult(result, 9);
                  case 0xd0:
                     // 1-byte signed int
                     result = buf.readInt8(offset + 1);
                     return buildDecodeResult(result, 2);
                  case 0xd1:
                     // 2-bytes signed int
                     result = buf.readInt16BE(offset + 1);
                     return buildDecodeResult(result, 3);
                  case 0xd2:
                     // 4-bytes signed int
                     result = buf.readInt32BE(offset + 1);
                     return buildDecodeResult(result, 5);
                  case 0xd3:
                     result = readInt64BE(buf.slice(offset + 1, offset + 9), 0);
                     return buildDecodeResult(result, 9);
                  case 0xca:
                     // 4-bytes float
                     result = buf.readFloatBE(offset + 1);
                     return buildDecodeResult(result, 5);
                  case 0xcb:
                     // 8-bytes double
                     result = buf.readDoubleBE(offset + 1);
                     return buildDecodeResult(result, 9);
                  case 0xd9:
                     // strings up to 2^8 - 1 bytes
                     length = buf.readUInt8(offset + 1);
                     if (!isValidDataSize(length, bufLength, 2)) {
                        return null;
                     }
                     result = buf.toString('utf8', offset + 2, offset + 2 + length);
                     return buildDecodeResult(result, 2 + length);
                  case 0xda:
                     // strings up to 2^16 - 2 bytes
                     length = buf.readUInt16BE(offset + 1);
                     if (!isValidDataSize(length, bufLength, 3)) {
                        return null;
                     }
                     result = buf.toString('utf8', offset + 3, offset + 3 + length);
                     return buildDecodeResult(result, 3 + length);
                  case 0xdb:
                     // strings up to 2^32 - 4 bytes
                     length = buf.readUInt32BE(offset + 1);
                     if (!isValidDataSize(length, bufLength, 5)) {
                        return null;
                     }
                     result = buf.toString('utf8', offset + 5, offset + 5 + length);
                     return buildDecodeResult(result, 5 + length);
                  case 0xc4:
                     // buffers up to 2^8 - 1 bytes
                     length = buf.readUInt8(offset + 1);
                     if (!isValidDataSize(length, bufLength, 2)) {
                        return null;
                     }
                     result = buf.slice(offset + 2, offset + 2 + length);
                     return buildDecodeResult(result, 2 + length);
                  case 0xc5:
                     // buffers up to 2^16 - 1 bytes
                     length = buf.readUInt16BE(offset + 1);
                     if (!isValidDataSize(length, bufLength, 3)) {
                        return null;
                     }
                     result = buf.slice(offset + 3, offset + 3 + length);
                     return buildDecodeResult(result, 3 + length);
                  case 0xc6:
                     // buffers up to 2^32 - 1 bytes
                     length = buf.readUInt32BE(offset + 1);
                     if (!isValidDataSize(length, bufLength, 5)) {
                        return null;
                     }
                     result = buf.slice(offset + 5, offset + 5 + length);
                     return buildDecodeResult(result, 5 + length);
                  case 0xdc:
                     // array up to 2^16 elements - 2 bytes
                     if (bufLength < 3) {
                        return null;
                     }

                     length = buf.readUInt16BE(offset + 1);
                     return decodeArray(buf, offset, length, 3);
                  case 0xdd:
                     // array up to 2^32 elements - 4 bytes
                     if (bufLength < 5) {
                        return null;
                     }

                     length = buf.readUInt32BE(offset + 1);
                     return decodeArray(buf, offset, length, 5);
                  case 0xde:
                     // maps up to 2^16 elements - 2 bytes
                     length = buf.readUInt16BE(offset + 1);
                     return decodeMap(buf, offset, length, 3);
                  case 0xdf:
                     throw new Error('map too big to decode in JS');
                  case 0xd4:
                     return decodeFixExt(buf, offset, 1);
                  case 0xd5:
                     return decodeFixExt(buf, offset, 2);
                  case 0xd6:
                     return decodeFixExt(buf, offset, 4);
                  case 0xd7:
                     return decodeFixExt(buf, offset, 8);
                  case 0xd8:
                     return decodeFixExt(buf, offset, 16);
                  case 0xc7:
                     // ext up to 2^8 - 1 bytes
                     length = buf.readUInt8(offset + 1);
                     type = buf.readUInt8(offset + 2);
                     if (!isValidDataSize(length, bufLength, 3)) {
                        return null;
                     }
                     return decodeExt(buf, offset, type, length, 3);
                  case 0xc8:
                     // ext up to 2^16 - 1 bytes
                     length = buf.readUInt16BE(offset + 1);
                     type = buf.readUInt8(offset + 3);
                     if (!isValidDataSize(length, bufLength, 4)) {
                        return null;
                     }
                     return decodeExt(buf, offset, type, length, 4);
                  case 0xc9:
                     // ext up to 2^32 - 1 bytes
                     length = buf.readUInt32BE(offset + 1);
                     type = buf.readUInt8(offset + 5);
                     if (!isValidDataSize(length, bufLength, 6)) {
                        return null;
                     }
                     return decodeExt(buf, offset, type, length, 6);
               }

               if ((first & 0xf0) === 0x90) {
                  // we have an array with less than 15 elements
                  length = first & 0x0f;
                  return decodeArray(buf, offset, length, 1);
               } else if ((first & 0xf0) === 0x80) {
                  // we have a map with less than 15 elements
                  length = first & 0x0f;
                  return decodeMap(buf, offset, length, 1);
               } else if ((first & 0xe0) === 0xa0) {
                  // fixstr up to 31 bytes
                  length = first & 0x1f;
                  if (isValidDataSize(length, bufLength, 1)) {
                     result = buf.toString('utf8', offset + 1, offset + length + 1);
                     return buildDecodeResult(result, length + 1);
                  } else {
                     return null;
                  }
               } else if (first >= 0xe0) {
                  // 5 bits negative ints
                  result = first - 0x100;
                  return buildDecodeResult(result, 1);
               } else if (first < 0x80) {
                  // 7-bits positive ints
                  return buildDecodeResult(first, 1);
               } else {
                  throw new Error('not implemented yet');
               }
            }

            function readInt64BE(buf, offset) {
               var negate = (buf[offset] & 0x80) == 0x80; // eslint-disable-line

               if (negate) {
                  var carry = 1;
                  for (var i = offset + 7; i >= offset; i--) {
                     var v = (buf[i] ^ 0xff) + carry;
                     buf[i] = v & 0xff;
                     carry = v >> 8;
                  }
               }

               var hi = buf.readUInt32BE(offset + 0);
               var lo = buf.readUInt32BE(offset + 4);
               return (hi * 4294967296 + lo) * (negate ? -1 : +1);
            }

            function decodeArray(buf, offset, length, headerLength) {
               var result = [];
               var i;
               var totalBytesConsumed = 0;

               offset += headerLength;
               for (i = 0; i < length; i++) {
                  var decodeResult = tryDecode(buf, offset);
                  if (decodeResult) {
                     result.push(decodeResult.value);
                     offset += decodeResult.bytesConsumed;
                     totalBytesConsumed += decodeResult.bytesConsumed;
                  } else {
                     return null;
                  }
               }
               return buildDecodeResult(result, headerLength + totalBytesConsumed);
            }

            function decodeMap(buf, offset, length, headerLength) {
               var result = {};
               var key;
               var i;
               var totalBytesConsumed = 0;

               offset += headerLength;
               for (i = 0; i < length; i++) {
                  var keyResult = tryDecode(buf, offset);
                  if (keyResult) {
                     offset += keyResult.bytesConsumed;
                     var valueResult = tryDecode(buf, offset);
                     if (valueResult) {
                        key = keyResult.value;
                        result[key] = valueResult.value;
                        offset += valueResult.bytesConsumed;
                        totalBytesConsumed += keyResult.bytesConsumed + valueResult.bytesConsumed;
                     } else {
                        return null;
                     }
                  } else {
                     return null;
                  }
               }
               return buildDecodeResult(result, headerLength + totalBytesConsumed);
            }

            function decodeFixExt(buf, offset, size) {
               var type = buf.readUInt8(offset + 1);

               return decodeExt(buf, offset, type, size, 2);
            }

            function decodeExt(buf, offset, type, size, headerSize) {
               var i, toDecode;

               offset += headerSize;
               for (i = 0; i < decodingTypes.length; i++) {
                  if (type === decodingTypes[i].type) {
                     toDecode = buf.slice(offset, offset + size);
                     var value = decodingTypes[i].decode(toDecode);
                     return buildDecodeResult(value, headerSize + size);
                  }
               }

               throw new Error('unable to find ext type ' + type);
            }
         };

         module.exports.IncompleteBufferError = IncompleteBufferError;

         /***/
      }),
/* 38 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

         var Buffer = __webpack_require__(7).Buffer;
         var bl = __webpack_require__(9);
         var TOLERANCE = 0.1;

         module.exports = function buildEncode(encodingTypes, forceFloat64, compatibilityMode) {
            function encode(obj, avoidSlice) {
               var buf, len;

               if (obj === undefined) {
                  throw new Error('undefined is not encodable in msgpack!');
               } else if (obj === null) {
                  buf = Buffer.allocUnsafe(1);
                  buf[0] = 0xc0;
               } else if (obj === true) {
                  buf = Buffer.allocUnsafe(1);
                  buf[0] = 0xc3;
               } else if (obj === false) {
                  buf = Buffer.allocUnsafe(1);
                  buf[0] = 0xc2;
               } else if (typeof obj === 'string') {
                  len = Buffer.byteLength(obj);
                  if (len < 32) {
                     buf = Buffer.allocUnsafe(1 + len);
                     buf[0] = 0xa0 | len;
                     if (len > 0) {
                        buf.write(obj, 1);
                     }
                  } else if (len <= 0xff && !compatibilityMode) {
                     // str8, but only when not in compatibility mode
                     buf = Buffer.allocUnsafe(2 + len);
                     buf[0] = 0xd9;
                     buf[1] = len;
                     buf.write(obj, 2);
                  } else if (len <= 0xffff) {
                     buf = Buffer.allocUnsafe(3 + len);
                     buf[0] = 0xda;
                     buf.writeUInt16BE(len, 1);
                     buf.write(obj, 3);
                  } else {
                     buf = Buffer.allocUnsafe(5 + len);
                     buf[0] = 0xdb;
                     buf.writeUInt32BE(len, 1);
                     buf.write(obj, 5);
                  }
               } else if (obj && obj.readUInt32LE) {
                  // weird hack to support Buffer
                  // and Buffer-like objects
                  if (obj.length <= 0xff) {
                     buf = Buffer.allocUnsafe(2);
                     buf[0] = 0xc4;
                     buf[1] = obj.length;
                  } else if (obj.length <= 0xffff) {
                     buf = Buffer.allocUnsafe(3);
                     buf[0] = 0xc5;
                     buf.writeUInt16BE(obj.length, 1);
                  } else {
                     buf = Buffer.allocUnsafe(5);
                     buf[0] = 0xc6;
                     buf.writeUInt32BE(obj.length, 1);
                  }

                  buf = bl([buf, obj]);
               } else if (Array.isArray(obj)) {
                  if (obj.length < 16) {
                     buf = Buffer.allocUnsafe(1);
                     buf[0] = 0x90 | obj.length;
                  } else if (obj.length < 65536) {
                     buf = Buffer.allocUnsafe(3);
                     buf[0] = 0xdc;
                     buf.writeUInt16BE(obj.length, 1);
                  } else {
                     buf = Buffer.allocUnsafe(5);
                     buf[0] = 0xdd;
                     buf.writeUInt32BE(obj.length, 1);
                  }

                  buf = obj.reduce(function (acc, obj) {
                     acc.append(encode(obj, true));
                     return acc;
                  }, bl().append(buf));
               } else if ((typeof obj === 'undefined' ? 'undefined' : _typeof(obj)) === 'object') {
                  buf = encodeExt(obj) || encodeObject(obj);
               } else if (typeof obj === 'number') {
                  if (isFloat(obj)) {
                     return encodeFloat(obj, forceFloat64);
                  } else if (obj >= 0) {
                     if (obj < 128) {
                        buf = Buffer.allocUnsafe(1);
                        buf[0] = obj;
                     } else if (obj < 256) {
                        buf = Buffer.allocUnsafe(2);
                        buf[0] = 0xcc;
                        buf[1] = obj;
                     } else if (obj < 65536) {
                        buf = Buffer.allocUnsafe(3);
                        buf[0] = 0xcd;
                        buf.writeUInt16BE(obj, 1);
                     } else if (obj <= 0xffffffff) {
                        buf = Buffer.allocUnsafe(5);
                        buf[0] = 0xce;
                        buf.writeUInt32BE(obj, 1);
                     } else if (obj <= 9007199254740991) {
                        buf = Buffer.allocUnsafe(9);
                        buf[0] = 0xcf;
                        write64BitUint(buf, obj);
                     } else {
                        return encodeFloat(obj, true);
                     }
                  } else {
                     if (obj >= -32) {
                        buf = Buffer.allocUnsafe(1);
                        buf[0] = 0x100 + obj;
                     } else if (obj >= -128) {
                        buf = Buffer.allocUnsafe(2);
                        buf[0] = 0xd0;
                        buf.writeInt8(obj, 1);
                     } else if (obj >= -32768) {
                        buf = Buffer.allocUnsafe(3);
                        buf[0] = 0xd1;
                        buf.writeInt16BE(obj, 1);
                     } else if (obj > -214748365) {
                        buf = Buffer.allocUnsafe(5);
                        buf[0] = 0xd2;
                        buf.writeInt32BE(obj, 1);
                     } else if (obj >= -9007199254740991) {
                        buf = Buffer.allocUnsafe(9);
                        buf[0] = 0xd3;
                        write64BitInt(buf, 1, obj);
                     } else {
                        return encodeFloat(obj, true);
                     }
                  }
               }

               if (!buf) {
                  throw new Error('not implemented yet');
               }

               if (avoidSlice) {
                  return buf;
               } else {
                  return buf.slice();
               }
            }

            function encodeExt(obj) {
               var i;
               var encoded;
               var length = -1;
               var headers = [];

               for (i = 0; i < encodingTypes.length; i++) {
                  if (encodingTypes[i].check(obj)) {
                     encoded = encodingTypes[i].encode(obj);
                     break;
                  }
               }

               if (!encoded) {
                  return null;
               }

               // we subtract 1 because the length does not
               // include the type
               length = encoded.length - 1;

               if (length === 1) {
                  headers.push(0xd4);
               } else if (length === 2) {
                  headers.push(0xd5);
               } else if (length === 4) {
                  headers.push(0xd6);
               } else if (length === 8) {
                  headers.push(0xd7);
               } else if (length === 16) {
                  headers.push(0xd8);
               } else if (length < 256) {
                  headers.push(0xc7);
                  headers.push(length);
               } else if (length < 0x10000) {
                  headers.push(0xc8);
                  headers.push(length >> 8);
                  headers.push(length & 0x00ff);
               } else {
                  headers.push(0xc9);
                  headers.push(length >> 24);
                  headers.push(length >> 16 & 0x000000ff);
                  headers.push(length >> 8 & 0x000000ff);
                  headers.push(length & 0x000000ff);
               }

               return bl().append(Buffer.from(headers)).append(encoded);
            }

            function encodeObject(obj) {
               var acc = [];
               var length = 0;
               var key;
               var header;

               for (key in obj) {
                  if (obj.hasOwnProperty(key) && obj[key] !== undefined && typeof obj[key] !== 'function') {
                     ++length;
                     acc.push(encode(key, true));
                     acc.push(encode(obj[key], true));
                  }
               }

               if (length < 16) {
                  header = Buffer.allocUnsafe(1);
                  header[0] = 0x80 | length;
               } else {
                  header = Buffer.allocUnsafe(3);
                  header[0] = 0xde;
                  header.writeUInt16BE(length, 1);
               }

               acc.unshift(header);

               var result = acc.reduce(function (list, buf) {
                  return list.append(buf);
               }, bl());

               return result;
            }

            return encode;
         };

         function write64BitUint(buf, obj) {
            // Write long byte by byte, in big-endian order
            for (var currByte = 7; currByte >= 0; currByte--) {
               buf[currByte + 1] = obj & 0xff;
               obj = obj / 256;
            }
         }

         function write64BitInt(buf, offset, num) {
            var negate = num < 0;

            if (negate) {
               num = Math.abs(num);
            }

            var lo = num % 4294967296;
            var hi = num / 4294967296;
            buf.writeUInt32BE(Math.floor(hi), offset + 0);
            buf.writeUInt32BE(lo, offset + 4);

            if (negate) {
               var carry = 1;
               for (var i = offset + 7; i >= offset; i--) {
                  var v = (buf[i] ^ 0xff) + carry;
                  buf[i] = v & 0xff;
                  carry = v >> 8;
               }
            }
         }

         function isFloat(n) {
            return n !== Math.floor(n);
         }

         function encodeFloat(obj, forceFloat64) {
            var buf;

            buf = Buffer.allocUnsafe(5);
            buf[0] = 0xca;
            buf.writeFloatBE(obj, 1);

            // FIXME is there a way to check if a
            // value fits in a float?
            if (forceFloat64 || Math.abs(obj - buf.readFloatBE(1)) > TOLERANCE) {
               buf = Buffer.allocUnsafe(9);
               buf[0] = 0xcb;
               buf.writeDoubleBE(obj, 1);
            }

            return buf;
         }

         /***/
      }),
/* 39 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var Transform = __webpack_require__(43).Transform;
         var inherits = __webpack_require__(2);
         var bl = __webpack_require__(9);

         function Base(opts) {
            opts = opts || {};

            opts.objectMode = true;
            opts.highWaterMark = 16;

            Transform.call(this, opts);

            this._msgpack = opts.msgpack;
         }

         inherits(Base, Transform);

         function Encoder(opts) {
            if (!(this instanceof Encoder)) {
               opts = opts || {};
               opts.msgpack = this;
               return new Encoder(opts);
            }

            Base.call(this, opts);
         }

         inherits(Encoder, Base);

         Encoder.prototype._transform = function (obj, enc, done) {
            var buf = null;

            try {
               buf = this._msgpack.encode(obj).slice(0);
            } catch (err) {
               this.emit('error', err);
               return done();
            }

            this.push(buf);
            done();
         };

         function Decoder(opts) {
            if (!(this instanceof Decoder)) {
               opts = opts || {};
               opts.msgpack = this;
               return new Decoder(opts);
            }

            Base.call(this, opts);

            this._chunks = bl();
         }

         inherits(Decoder, Base);

         Decoder.prototype._transform = function (buf, enc, done) {
            if (buf) {
               this._chunks.append(buf);
            }

            try {
               var result = this._msgpack.decode(this._chunks);
               this.push(result);
            } catch (err) {
               if (err instanceof this._msgpack.IncompleteBufferError) {
                  done();
               } else {
                  this.emit('error', err);
               }
               return;
            }

            if (this._chunks.length > 0) {
               this._transform(null, enc, done);
            } else {
               done();
            }
         };

         module.exports.decoder = Decoder;
         module.exports.encoder = Encoder;

         /***/
      }),
/* 40 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         module.exports = __webpack_require__(0);

         /***/
      }),
/* 41 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
         // Copyright Joyent, Inc. and other Node contributors.
         //
         // Permission is hereby granted, free of charge, to any person obtaining a
         // copy of this software and associated documentation files (the
         // "Software"), to deal in the Software without restriction, including
         // without limitation the rights to use, copy, modify, merge, publish,
         // distribute, sublicense, and/or sell copies of the Software, and to permit
         // persons to whom the Software is furnished to do so, subject to the
         // following conditions:
         //
         // The above copyright notice and this permission notice shall be included
         // in all copies or substantial portions of the Software.
         //
         // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
         // OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
         // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
         // NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
         // DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
         // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
         // USE OR OTHER DEALINGS IN THE SOFTWARE.

         // a passthrough stream.
         // basically just the most minimal sort of Transform stream.
         // Every written chunk gets output as-is.



         module.exports = PassThrough;

         var Transform = __webpack_require__(23);

         /*<replacement>*/
         var util = __webpack_require__(5);
         util.inherits = __webpack_require__(2);
         /*</replacement>*/

         util.inherits(PassThrough, Transform);

         function PassThrough(options) {
            if (!(this instanceof PassThrough)) return new PassThrough(options);

            Transform.call(this, options);
         }

         PassThrough.prototype._transform = function (chunk, encoding, cb) {
            cb(null, chunk);
         };

         /***/
      }),
/* 42 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         /*<replacement>*/

         function _classCallCheck(instance, Constructor) {
            if (!(instance instanceof Constructor)) {
               throw new TypeError("Cannot call a class as a function");
            }
         }

         var Buffer = __webpack_require__(7).Buffer;
         /*</replacement>*/

         function copyBuffer(src, target, offset) {
            src.copy(target, offset);
         }

         module.exports = function () {
            function BufferList() {
               _classCallCheck(this, BufferList);

               this.head = null;
               this.tail = null;
               this.length = 0;
            }

            BufferList.prototype.push = function push(v) {
               var entry = { data: v, next: null };
               if (this.length > 0) this.tail.next = entry; else this.head = entry;
               this.tail = entry;
               ++this.length;
            };

            BufferList.prototype.unshift = function unshift(v) {
               var entry = { data: v, next: this.head };
               if (this.length === 0) this.tail = entry;
               this.head = entry;
               ++this.length;
            };

            BufferList.prototype.shift = function shift() {
               if (this.length === 0) return;
               var ret = this.head.data;
               if (this.length === 1) this.head = this.tail = null; else this.head = this.head.next;
               --this.length;
               return ret;
            };

            BufferList.prototype.clear = function clear() {
               this.head = this.tail = null;
               this.length = 0;
            };

            BufferList.prototype.join = function join(s) {
               if (this.length === 0) return '';
               var p = this.head;
               var ret = '' + p.data;
               while (p = p.next) {
                  ret += s + p.data;
               } return ret;
            };

            BufferList.prototype.concat = function concat(n) {
               if (this.length === 0) return Buffer.alloc(0);
               if (this.length === 1) return this.head.data;
               var ret = Buffer.allocUnsafe(n >>> 0);
               var p = this.head;
               var i = 0;
               while (p) {
                  copyBuffer(p.data, ret, i);
                  i += p.data.length;
                  p = p.next;
               }
               return ret;
            };

            return BufferList;
         }();

         /***/
      }),
/* 43 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         exports = module.exports = __webpack_require__(22);
         exports.Stream = exports;
         exports.Readable = exports;
         exports.Writable = __webpack_require__(24);
         exports.Duplex = __webpack_require__(0);
         exports.Transform = __webpack_require__(23);
         exports.PassThrough = __webpack_require__(41);

         /***/
      }),
/* 44 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (module) {

            var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

            /**
             * Copyright (c) 2014, Facebook, Inc.
             * All rights reserved.
             *
             * This source code is licensed under the BSD-style license found in the
             * https://raw.github.com/facebook/regenerator/master/LICENSE file. An
             * additional grant of patent rights can be found in the PATENTS file in
             * the same directory.
             */

            !function (global) {
               "use strict";

               var Op = Object.prototype;
               var hasOwn = Op.hasOwnProperty;
               var undefined; // More compressible than void 0.
               var $Symbol = typeof Symbol === "function" ? Symbol : {};
               var iteratorSymbol = $Symbol.iterator || "@@iterator";
               var asyncIteratorSymbol = $Symbol.asyncIterator || "@@asyncIterator";
               var toStringTagSymbol = $Symbol.toStringTag || "@@toStringTag";

               var inModule = (false ? "undefined" : _typeof(module)) === "object";
               var runtime = global.regeneratorRuntime;
               if (runtime) {
                  if (inModule) {
                     // If regeneratorRuntime is defined globally and we're in a module,
                     // make the exports object identical to regeneratorRuntime.
                     module.exports = runtime;
                  }
                  // Don't bother evaluating the rest of this file if the runtime was
                  // already defined globally.
                  return;
               }

               // Define the runtime globally (as expected by generated code) as either
               // module.exports (if we're in a module) or a new, empty object.
               runtime = global.regeneratorRuntime = inModule ? module.exports : {};

               function wrap(innerFn, outerFn, self, tryLocsList) {
                  // If outerFn provided and outerFn.prototype is a Generator, then outerFn.prototype instanceof Generator.
                  var protoGenerator = outerFn && outerFn.prototype instanceof Generator ? outerFn : Generator;
                  var generator = Object.create(protoGenerator.prototype);
                  var context = new Context(tryLocsList || []);

                  // The ._invoke method unifies the implementations of the .next,
                  // .throw, and .return methods.
                  generator._invoke = makeInvokeMethod(innerFn, self, context);

                  return generator;
               }
               runtime.wrap = wrap;

               // Try/catch helper to minimize deoptimizations. Returns a completion
               // record like context.tryEntries[i].completion. This interface could
               // have been (and was previously) designed to take a closure to be
               // invoked without arguments, but in all the cases we care about we
               // already have an existing method we want to call, so there's no need
               // to create a new function object. We can even get away with assuming
               // the method takes exactly one argument, since that happens to be true
               // in every case, so we don't have to touch the arguments object. The
               // only additional allocation required is the completion record, which
               // has a stable shape and so hopefully should be cheap to allocate.
               function tryCatch(fn, obj, arg) {
                  try {
                     return { type: "normal", arg: fn.call(obj, arg) };
                  } catch (err) {
                     return { type: "throw", arg: err };
                  }
               }

               var GenStateSuspendedStart = "suspendedStart";
               var GenStateSuspendedYield = "suspendedYield";
               var GenStateExecuting = "executing";
               var GenStateCompleted = "completed";

               // Returning this object from the innerFn has the same effect as
               // breaking out of the dispatch switch statement.
               var ContinueSentinel = {};

               // Dummy constructor functions that we use as the .constructor and
               // .constructor.prototype properties for functions that return Generator
               // objects. For full spec compliance, you may wish to configure your
               // minifier not to mangle the names of these two functions.
               function Generator() { }
               function GeneratorFunction() { }
               function GeneratorFunctionPrototype() { }

               // This is a polyfill for %IteratorPrototype% for environments that
               // don't natively support it.
               var IteratorPrototype = {};
               IteratorPrototype[iteratorSymbol] = function () {
                  return this;
               };

               var getProto = Object.getPrototypeOf;
               var NativeIteratorPrototype = getProto && getProto(getProto(values([])));
               if (NativeIteratorPrototype && NativeIteratorPrototype !== Op && hasOwn.call(NativeIteratorPrototype, iteratorSymbol)) {
                  // This environment has a native %IteratorPrototype%; use it instead
                  // of the polyfill.
                  IteratorPrototype = NativeIteratorPrototype;
               }

               var Gp = GeneratorFunctionPrototype.prototype = Generator.prototype = Object.create(IteratorPrototype);
               GeneratorFunction.prototype = Gp.constructor = GeneratorFunctionPrototype;
               GeneratorFunctionPrototype.constructor = GeneratorFunction;
               GeneratorFunctionPrototype[toStringTagSymbol] = GeneratorFunction.displayName = "GeneratorFunction";

               // Helper for defining the .next, .throw, and .return methods of the
               // Iterator interface in terms of a single ._invoke method.
               function defineIteratorMethods(prototype) {
                  ["next", "throw", "return"].forEach(function (method) {
                     prototype[method] = function (arg) {
                        return this._invoke(method, arg);
                     };
                  });
               }

               runtime.isGeneratorFunction = function (genFun) {
                  var ctor = typeof genFun === "function" && genFun.constructor;
                  return ctor ? ctor === GeneratorFunction ||
                     // For the native GeneratorFunction constructor, the best we can
                     // do is to check its .name property.
                     (ctor.displayName || ctor.name) === "GeneratorFunction" : false;
               };

               runtime.mark = function (genFun) {
                  if (Object.setPrototypeOf) {
                     Object.setPrototypeOf(genFun, GeneratorFunctionPrototype);
                  } else {
                     genFun.__proto__ = GeneratorFunctionPrototype;
                     if (!(toStringTagSymbol in genFun)) {
                        genFun[toStringTagSymbol] = "GeneratorFunction";
                     }
                  }
                  genFun.prototype = Object.create(Gp);
                  return genFun;
               };

               // Within the body of any async function, `await x` is transformed to
               // `yield regeneratorRuntime.awrap(x)`, so that the runtime can test
               // `hasOwn.call(value, "__await")` to determine if the yielded value is
               // meant to be awaited.
               runtime.awrap = function (arg) {
                  return { __await: arg };
               };

               function AsyncIterator(generator) {
                  function invoke(method, arg, resolve, reject) {
                     var record = tryCatch(generator[method], generator, arg);
                     if (record.type === "throw") {
                        reject(record.arg);
                     } else {
                        var result = record.arg;
                        var value = result.value;
                        if (value && (typeof value === "undefined" ? "undefined" : _typeof(value)) === "object" && hasOwn.call(value, "__await")) {
                           return Promise.resolve(value.__await).then(function (value) {
                              invoke("next", value, resolve, reject);
                           }, function (err) {
                              invoke("throw", err, resolve, reject);
                           });
                        }

                        return Promise.resolve(value).then(function (unwrapped) {
                           // When a yielded Promise is resolved, its final value becomes
                           // the .value of the Promise<{value,done}> result for the
                           // current iteration. If the Promise is rejected, however, the
                           // result for this iteration will be rejected with the same
                           // reason. Note that rejections of yielded Promises are not
                           // thrown back into the generator function, as is the case
                           // when an awaited Promise is rejected. This difference in
                           // behavior between yield and await is important, because it
                           // allows the consumer to decide what to do with the yielded
                           // rejection (swallow it and continue, manually .throw it back
                           // into the generator, abandon iteration, whatever). With
                           // await, by contrast, there is no opportunity to examine the
                           // rejection reason outside the generator function, so the
                           // only option is to throw it from the await expression, and
                           // let the generator function handle the exception.
                           result.value = unwrapped;
                           resolve(result);
                        }, reject);
                     }
                  }

                  var previousPromise;

                  function enqueue(method, arg) {
                     function callInvokeWithMethodAndArg() {
                        return new Promise(function (resolve, reject) {
                           invoke(method, arg, resolve, reject);
                        });
                     }

                     return previousPromise =
                        // If enqueue has been called before, then we want to wait until
                        // all previous Promises have been resolved before calling invoke,
                        // so that results are always delivered in the correct order. If
                        // enqueue has not been called before, then it is important to
                        // call invoke immediately, without waiting on a callback to fire,
                        // so that the async generator function has the opportunity to do
                        // any necessary setup in a predictable way. This predictability
                        // is why the Promise constructor synchronously invokes its
                        // executor callback, and why async functions synchronously
                        // execute code before the first await. Since we implement simple
                        // async functions in terms of async generators, it is especially
                        // important to get this right, even though it requires care.
                        previousPromise ? previousPromise.then(callInvokeWithMethodAndArg,
                           // Avoid propagating failures to Promises returned by later
                           // invocations of the iterator.
                           callInvokeWithMethodAndArg) : callInvokeWithMethodAndArg();
                  }

                  // Define the unified helper method that is used to implement .next,
                  // .throw, and .return (see defineIteratorMethods).
                  this._invoke = enqueue;
               }

               defineIteratorMethods(AsyncIterator.prototype);
               AsyncIterator.prototype[asyncIteratorSymbol] = function () {
                  return this;
               };
               runtime.AsyncIterator = AsyncIterator;

               // Note that simple async functions are implemented on top of
               // AsyncIterator objects; they just return a Promise for the value of
               // the final result produced by the iterator.
               runtime.async = function (innerFn, outerFn, self, tryLocsList) {
                  var iter = new AsyncIterator(wrap(innerFn, outerFn, self, tryLocsList));

                  return runtime.isGeneratorFunction(outerFn) ? iter // If outerFn is a generator, return the full iterator.
                     : iter.next().then(function (result) {
                        return result.done ? result.value : iter.next();
                     });
               };

               function makeInvokeMethod(innerFn, self, context) {
                  var state = GenStateSuspendedStart;

                  return function invoke(method, arg) {
                     if (state === GenStateExecuting) {
                        throw new Error("Generator is already running");
                     }

                     if (state === GenStateCompleted) {
                        if (method === "throw") {
                           throw arg;
                        }

                        // Be forgiving, per 25.3.3.3.3 of the spec:
                        // https://people.mozilla.org/~jorendorff/es6-draft.html#sec-generatorresume
                        return doneResult();
                     }

                     context.method = method;
                     context.arg = arg;

                     while (true) {
                        var delegate = context.delegate;
                        if (delegate) {
                           var delegateResult = maybeInvokeDelegate(delegate, context);
                           if (delegateResult) {
                              if (delegateResult === ContinueSentinel) continue;
                              return delegateResult;
                           }
                        }

                        if (context.method === "next") {
                           // Setting context._sent for legacy support of Babel's
                           // function.sent implementation.
                           context.sent = context._sent = context.arg;
                        } else if (context.method === "throw") {
                           if (state === GenStateSuspendedStart) {
                              state = GenStateCompleted;
                              throw context.arg;
                           }

                           context.dispatchException(context.arg);
                        } else if (context.method === "return") {
                           context.abrupt("return", context.arg);
                        }

                        state = GenStateExecuting;

                        var record = tryCatch(innerFn, self, context);
                        if (record.type === "normal") {
                           // If an exception is thrown from innerFn, we leave state ===
                           // GenStateExecuting and loop back for another invocation.
                           state = context.done ? GenStateCompleted : GenStateSuspendedYield;

                           if (record.arg === ContinueSentinel) {
                              continue;
                           }

                           return {
                              value: record.arg,
                              done: context.done
                           };
                        } else if (record.type === "throw") {
                           state = GenStateCompleted;
                           // Dispatch the exception by looping back around to the
                           // context.dispatchException(context.arg) call above.
                           context.method = "throw";
                           context.arg = record.arg;
                        }
                     }
                  };
               }

               // Call delegate.iterator[context.method](context.arg) and handle the
               // result, either by returning a { value, done } result from the
               // delegate iterator, or by modifying context.method and context.arg,
               // setting context.delegate to null, and returning the ContinueSentinel.
               function maybeInvokeDelegate(delegate, context) {
                  var method = delegate.iterator[context.method];
                  if (method === undefined) {
                     // A .throw or .return when the delegate iterator has no .throw
                     // method always terminates the yield* loop.
                     context.delegate = null;

                     if (context.method === "throw") {
                        if (delegate.iterator.return) {
                           // If the delegate iterator has a return method, give it a
                           // chance to clean up.
                           context.method = "return";
                           context.arg = undefined;
                           maybeInvokeDelegate(delegate, context);

                           if (context.method === "throw") {
                              // If maybeInvokeDelegate(context) changed context.method from
                              // "return" to "throw", let that override the TypeError below.
                              return ContinueSentinel;
                           }
                        }

                        context.method = "throw";
                        context.arg = new TypeError("The iterator does not provide a 'throw' method");
                     }

                     return ContinueSentinel;
                  }

                  var record = tryCatch(method, delegate.iterator, context.arg);

                  if (record.type === "throw") {
                     context.method = "throw";
                     context.arg = record.arg;
                     context.delegate = null;
                     return ContinueSentinel;
                  }

                  var info = record.arg;

                  if (!info) {
                     context.method = "throw";
                     context.arg = new TypeError("iterator result is not an object");
                     context.delegate = null;
                     return ContinueSentinel;
                  }

                  if (info.done) {
                     // Assign the result of the finished delegate to the temporary
                     // variable specified by delegate.resultName (see delegateYield).
                     context[delegate.resultName] = info.value;

                     // Resume execution at the desired location (see delegateYield).
                     context.next = delegate.nextLoc;

                     // If context.method was "throw" but the delegate handled the
                     // exception, let the outer generator proceed normally. If
                     // context.method was "next", forget context.arg since it has been
                     // "consumed" by the delegate iterator. If context.method was
                     // "return", allow the original .return call to continue in the
                     // outer generator.
                     if (context.method !== "return") {
                        context.method = "next";
                        context.arg = undefined;
                     }
                  } else {
                     // Re-yield the result returned by the delegate method.
                     return info;
                  }

                  // The delegate iterator is finished, so forget it and continue with
                  // the outer generator.
                  context.delegate = null;
                  return ContinueSentinel;
               }

               // Define Generator.prototype.{next,throw,return} in terms of the
               // unified ._invoke helper method.
               defineIteratorMethods(Gp);

               Gp[toStringTagSymbol] = "Generator";

               // A Generator should always return itself as the iterator object when the
               // @@iterator function is called on it. Some browsers' implementations of the
               // iterator prototype chain incorrectly implement this, causing the Generator
               // object to not be returned from this call. This ensures that doesn't happen.
               // See https://github.com/facebook/regenerator/issues/274 for more details.
               Gp[iteratorSymbol] = function () {
                  return this;
               };

               Gp.toString = function () {
                  return "[object Generator]";
               };

               function pushTryEntry(locs) {
                  var entry = { tryLoc: locs[0] };

                  if (1 in locs) {
                     entry.catchLoc = locs[1];
                  }

                  if (2 in locs) {
                     entry.finallyLoc = locs[2];
                     entry.afterLoc = locs[3];
                  }

                  this.tryEntries.push(entry);
               }

               function resetTryEntry(entry) {
                  var record = entry.completion || {};
                  record.type = "normal";
                  delete record.arg;
                  entry.completion = record;
               }

               function Context(tryLocsList) {
                  // The root entry object (effectively a try statement without a catch
                  // or a finally block) gives us a place to store values thrown from
                  // locations where there is no enclosing try statement.
                  this.tryEntries = [{ tryLoc: "root" }];
                  tryLocsList.forEach(pushTryEntry, this);
                  this.reset(true);
               }

               runtime.keys = function (object) {
                  var keys = [];
                  for (var key in object) {
                     keys.push(key);
                  }
                  keys.reverse();

                  // Rather than returning an object with a next method, we keep
                  // things simple and return the next function itself.
                  return function next() {
                     while (keys.length) {
                        var key = keys.pop();
                        if (key in object) {
                           next.value = key;
                           next.done = false;
                           return next;
                        }
                     }

                     // To avoid creating an additional object, we just hang the .value
                     // and .done properties off the next function object itself. This
                     // also ensures that the minifier will not anonymize the function.
                     next.done = true;
                     return next;
                  };
               };

               function values(iterable) {
                  if (iterable) {
                     var iteratorMethod = iterable[iteratorSymbol];
                     if (iteratorMethod) {
                        return iteratorMethod.call(iterable);
                     }

                     if (typeof iterable.next === "function") {
                        return iterable;
                     }

                     if (!isNaN(iterable.length)) {
                        var i = -1,
                           next = function next() {
                              while (++i < iterable.length) {
                                 if (hasOwn.call(iterable, i)) {
                                    next.value = iterable[i];
                                    next.done = false;
                                    return next;
                                 }
                              }

                              next.value = undefined;
                              next.done = true;

                              return next;
                           };

                        return next.next = next;
                     }
                  }

                  // Return an iterator with no values.
                  return { next: doneResult };
               }
               runtime.values = values;

               function doneResult() {
                  return { value: undefined, done: true };
               }

               Context.prototype = {
                  constructor: Context,

                  reset: function reset(skipTempReset) {
                     this.prev = 0;
                     this.next = 0;
                     // Resetting context._sent for legacy support of Babel's
                     // function.sent implementation.
                     this.sent = this._sent = undefined;
                     this.done = false;
                     this.delegate = null;

                     this.method = "next";
                     this.arg = undefined;

                     this.tryEntries.forEach(resetTryEntry);

                     if (!skipTempReset) {
                        for (var name in this) {
                           // Not sure about the optimal order of these conditions:
                           if (name.charAt(0) === "t" && hasOwn.call(this, name) && !isNaN(+name.slice(1))) {
                              this[name] = undefined;
                           }
                        }
                     }
                  },

                  stop: function stop() {
                     this.done = true;

                     var rootEntry = this.tryEntries[0];
                     var rootRecord = rootEntry.completion;
                     if (rootRecord.type === "throw") {
                        throw rootRecord.arg;
                     }

                     return this.rval;
                  },

                  dispatchException: function dispatchException(exception) {
                     if (this.done) {
                        throw exception;
                     }

                     var context = this;
                     function handle(loc, caught) {
                        record.type = "throw";
                        record.arg = exception;
                        context.next = loc;

                        if (caught) {
                           // If the dispatched exception was caught by a catch block,
                           // then let that catch block handle the exception normally.
                           context.method = "next";
                           context.arg = undefined;
                        }

                        return !!caught;
                     }

                     for (var i = this.tryEntries.length - 1; i >= 0; --i) {
                        var entry = this.tryEntries[i];
                        var record = entry.completion;

                        if (entry.tryLoc === "root") {
                           // Exception thrown outside of any try block that could handle
                           // it, so set the completion value of the entire function to
                           // throw the exception.
                           return handle("end");
                        }

                        if (entry.tryLoc <= this.prev) {
                           var hasCatch = hasOwn.call(entry, "catchLoc");
                           var hasFinally = hasOwn.call(entry, "finallyLoc");

                           if (hasCatch && hasFinally) {
                              if (this.prev < entry.catchLoc) {
                                 return handle(entry.catchLoc, true);
                              } else if (this.prev < entry.finallyLoc) {
                                 return handle(entry.finallyLoc);
                              }
                           } else if (hasCatch) {
                              if (this.prev < entry.catchLoc) {
                                 return handle(entry.catchLoc, true);
                              }
                           } else if (hasFinally) {
                              if (this.prev < entry.finallyLoc) {
                                 return handle(entry.finallyLoc);
                              }
                           } else {
                              throw new Error("try statement without catch or finally");
                           }
                        }
                     }
                  },

                  abrupt: function abrupt(type, arg) {
                     for (var i = this.tryEntries.length - 1; i >= 0; --i) {
                        var entry = this.tryEntries[i];
                        if (entry.tryLoc <= this.prev && hasOwn.call(entry, "finallyLoc") && this.prev < entry.finallyLoc) {
                           var finallyEntry = entry;
                           break;
                        }
                     }

                     if (finallyEntry && (type === "break" || type === "continue") && finallyEntry.tryLoc <= arg && arg <= finallyEntry.finallyLoc) {
                        // Ignore the finally entry if control is not jumping to a
                        // location outside the try/catch block.
                        finallyEntry = null;
                     }

                     var record = finallyEntry ? finallyEntry.completion : {};
                     record.type = type;
                     record.arg = arg;

                     if (finallyEntry) {
                        this.method = "next";
                        this.next = finallyEntry.finallyLoc;
                        return ContinueSentinel;
                     }

                     return this.complete(record);
                  },

                  complete: function complete(record, afterLoc) {
                     if (record.type === "throw") {
                        throw record.arg;
                     }

                     if (record.type === "break" || record.type === "continue") {
                        this.next = record.arg;
                     } else if (record.type === "return") {
                        this.rval = this.arg = record.arg;
                        this.method = "return";
                        this.next = "end";
                     } else if (record.type === "normal" && afterLoc) {
                        this.next = afterLoc;
                     }

                     return ContinueSentinel;
                  },

                  finish: function finish(finallyLoc) {
                     for (var i = this.tryEntries.length - 1; i >= 0; --i) {
                        var entry = this.tryEntries[i];
                        if (entry.finallyLoc === finallyLoc) {
                           this.complete(entry.completion, entry.afterLoc);
                           resetTryEntry(entry);
                           return ContinueSentinel;
                        }
                     }
                  },

                  "catch": function _catch(tryLoc) {
                     for (var i = this.tryEntries.length - 1; i >= 0; --i) {
                        var entry = this.tryEntries[i];
                        if (entry.tryLoc === tryLoc) {
                           var record = entry.completion;
                           if (record.type === "throw") {
                              var thrown = record.arg;
                              resetTryEntry(entry);
                           }
                           return thrown;
                        }
                     }

                     // The context.catch method must only be called with a location
                     // argument that corresponds to a known catch block.
                     throw new Error("illegal catch attempt");
                  },

                  delegateYield: function delegateYield(iterable, resultName, nextLoc) {
                     this.delegate = {
                        iterator: values(iterable),
                        resultName: resultName,
                        nextLoc: nextLoc
                     };

                     if (this.method === "next") {
                        // Deliberately forget the last sent value so that we don't
                        // accidentally pass it on to the delegate.
                        this.arg = undefined;
                     }

                     return ContinueSentinel;
                  }
               };
            }(
               // In sloppy mode, unbound `this` refers to the global object, fallback to
               // Function constructor if we're in global strict mode. That is sadly a form
               // of indirect eval which violates Content Security Policy.
               function () {
                  return this;
               }() || Function("return this")());
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(50)(module)))

         /***/
      }),
/* 45 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global, process) {

            (function (global, undefined) {
               "use strict";

               if (global.setImmediate) {
                  return;
               }

               var nextHandle = 1; // Spec says greater than zero
               var tasksByHandle = {};
               var currentlyRunningATask = false;
               var doc = global.document;
               var registerImmediate;

               function setImmediate(callback) {
                  // Callback can either be a function or a string
                  if (typeof callback !== "function") {
                     callback = new Function("" + callback);
                  }
                  // Copy function arguments
                  var args = new Array(arguments.length - 1);
                  for (var i = 0; i < args.length; i++) {
                     args[i] = arguments[i + 1];
                  }
                  // Store and register the task
                  var task = { callback: callback, args: args };
                  tasksByHandle[nextHandle] = task;
                  registerImmediate(nextHandle);
                  return nextHandle++;
               }

               function clearImmediate(handle) {
                  delete tasksByHandle[handle];
               }

               function run(task) {
                  var callback = task.callback;
                  var args = task.args;
                  switch (args.length) {
                     case 0:
                        callback();
                        break;
                     case 1:
                        callback(args[0]);
                        break;
                     case 2:
                        callback(args[0], args[1]);
                        break;
                     case 3:
                        callback(args[0], args[1], args[2]);
                        break;
                     default:
                        callback.apply(undefined, args);
                        break;
                  }
               }

               function runIfPresent(handle) {
                  // From the spec: "Wait until any invocations of this algorithm started before this one have completed."
                  // So if we're currently running a task, we'll need to delay this invocation.
                  if (currentlyRunningATask) {
                     // Delay by doing a setTimeout. setImmediate was tried instead, but in Firefox 7 it generated a
                     // "too much recursion" error.
                     setTimeout(runIfPresent, 0, handle);
                  } else {
                     var task = tasksByHandle[handle];
                     if (task) {
                        currentlyRunningATask = true;
                        try {
                           run(task);
                        } finally {
                           clearImmediate(handle);
                           currentlyRunningATask = false;
                        }
                     }
                  }
               }

               function installNextTickImplementation() {
                  registerImmediate = function registerImmediate(handle) {
                     process.nextTick(function () {
                        runIfPresent(handle);
                     });
                  };
               }

               function canUsePostMessage() {
                  // The test against `importScripts` prevents this implementation from being installed inside a web worker,
                  // where `global.postMessage` means something completely different and can't be used for this purpose.
                  if (global.postMessage && !global.importScripts) {
                     var postMessageIsAsynchronous = true;
                     var oldOnMessage = global.onmessage;
                     global.onmessage = function () {
                        postMessageIsAsynchronous = false;
                     };
                     global.postMessage("", "*");
                     global.onmessage = oldOnMessage;
                     return postMessageIsAsynchronous;
                  }
               }

               function installPostMessageImplementation() {
                  // Installs an event handler on `global` for the `message` event: see
                  // * https://developer.mozilla.org/en/DOM/window.postMessage
                  // * http://www.whatwg.org/specs/web-apps/current-work/multipage/comms.html#crossDocumentMessages

                  var messagePrefix = "setImmediate$" + Math.random() + "$";
                  var onGlobalMessage = function onGlobalMessage(event) {
                     if (event.source === global && typeof event.data === "string" && event.data.indexOf(messagePrefix) === 0) {
                        runIfPresent(+event.data.slice(messagePrefix.length));
                     }
                  };

                  if (global.addEventListener) {
                     global.addEventListener("message", onGlobalMessage, false);
                  } else {
                     global.attachEvent("onmessage", onGlobalMessage);
                  }

                  registerImmediate = function registerImmediate(handle) {
                     global.postMessage(messagePrefix + handle, "*");
                  };
               }

               function installMessageChannelImplementation() {
                  var channel = new MessageChannel();
                  channel.port1.onmessage = function (event) {
                     var handle = event.data;
                     runIfPresent(handle);
                  };

                  registerImmediate = function registerImmediate(handle) {
                     channel.port2.postMessage(handle);
                  };
               }

               function installReadyStateChangeImplementation() {
                  var html = doc.documentElement;
                  registerImmediate = function registerImmediate(handle) {
                     // Create a <script> element; its readystatechange event will be fired asynchronously once it is inserted
                     // into the document. Do so, thus queuing up the task. Remember to clean up once it's been called.
                     var script = doc.createElement("script");
                     script.onreadystatechange = function () {
                        runIfPresent(handle);
                        script.onreadystatechange = null;
                        html.removeChild(script);
                        script = null;
                     };
                     html.appendChild(script);
                  };
               }

               function installSetTimeoutImplementation() {
                  registerImmediate = function registerImmediate(handle) {
                     setTimeout(runIfPresent, 0, handle);
                  };
               }

               // If supported, we should attach to the prototype of global, since that is where setTimeout et al. live.
               var attachTo = Object.getPrototypeOf && Object.getPrototypeOf(global);
               attachTo = attachTo && attachTo.setTimeout ? attachTo : global;

               // Don't get fooled by e.g. browserify environments.
               if ({}.toString.call(global.process) === "[object process]") {
                  // For Node.js before 0.9
                  installNextTickImplementation();
               } else if (canUsePostMessage()) {
                  // For non-IE10 modern browsers
                  installPostMessageImplementation();
               } else if (global.MessageChannel) {
                  // For web workers, where supported
                  installMessageChannelImplementation();
               } else if (doc && "onreadystatechange" in doc.createElement("script")) {
                  // For IE 6–8
                  installReadyStateChangeImplementation();
               } else {
                  // For older browsers
                  installSetTimeoutImplementation();
               }

               attachTo.setImmediate = setImmediate;
               attachTo.clearImmediate = clearImmediate;
            })(typeof self === "undefined" ? typeof global === "undefined" ? undefined : global : self);
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1), __webpack_require__(6)))

         /***/
      }),
/* 46 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var apply = Function.prototype.apply;

         // DOM APIs, for completeness

         exports.setTimeout = function () {
            return new Timeout(apply.call(setTimeout, window, arguments), clearTimeout);
         };
         exports.setInterval = function () {
            return new Timeout(apply.call(setInterval, window, arguments), clearInterval);
         };
         exports.clearTimeout = exports.clearInterval = function (timeout) {
            if (timeout) {
               timeout.close();
            }
         };

         function Timeout(id, clearFn) {
            this._id = id;
            this._clearFn = clearFn;
         }
         Timeout.prototype.unref = Timeout.prototype.ref = function () { };
         Timeout.prototype.close = function () {
            this._clearFn.call(window, this._id);
         };

         // Does not start the time, just sets up the members needed.
         exports.enroll = function (item, msecs) {
            clearTimeout(item._idleTimeoutId);
            item._idleTimeout = msecs;
         };

         exports.unenroll = function (item) {
            clearTimeout(item._idleTimeoutId);
            item._idleTimeout = -1;
         };

         exports._unrefActive = exports.active = function (item) {
            clearTimeout(item._idleTimeoutId);

            var msecs = item._idleTimeout;
            if (msecs >= 0) {
               item._idleTimeoutId = setTimeout(function onTimeout() {
                  if (item._onTimeout) item._onTimeout();
               }, msecs);
            }
         };

         // setimmediate attaches itself to the global object
         __webpack_require__(45);
         exports.setImmediate = setImmediate;
         exports.clearImmediate = clearImmediate;

         /***/
      }),
/* 47 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";
/* WEBPACK VAR INJECTION */(function (global) {

            /**
             * Module exports.
             */

            module.exports = deprecate;

            /**
             * Mark that a method should not be used.
             * Returns a modified function which warns once by default.
             *
             * If `localStorage.noDeprecation = true` is set, then it is a no-op.
             *
             * If `localStorage.throwDeprecation = true` is set, then deprecated functions
             * will throw an Error when invoked.
             *
             * If `localStorage.traceDeprecation = true` is set, then deprecated functions
             * will invoke `console.trace()` instead of `console.error()`.
             *
             * @param {Function} fn - the function to deprecate
             * @param {String} msg - the string to print to the console when `fn` is invoked
             * @returns {Function} a new "deprecated" version of `fn`
             * @api public
             */

            function deprecate(fn, msg) {
               if (config('noDeprecation')) {
                  return fn;
               }

               var warned = false;
               function deprecated() {
                  if (!warned) {
                     if (config('throwDeprecation')) {
                        throw new Error(msg);
                     } else if (config('traceDeprecation')) {
                        console.trace(msg);
                     } else {
                        console.warn(msg);
                     }
                     warned = true;
                  }
                  return fn.apply(this, arguments);
               }

               return deprecated;
            }

            /**
             * Checks `localStorage` for boolean values for the given `name`.
             *
             * @param {String} name
             * @returns {Boolean}
             * @api private
             */

            function config(name) {
               // accessing global.localStorage can trigger a DOMException in sandboxed iframes
               try {
                  if (!global.localStorage) return false;
               } catch (_) {
                  return false;
               }
               var val = global.localStorage[name];
               if (null == val) return false;
               return String(val).toLowerCase() === 'true';
            }
            /* WEBPACK VAR INJECTION */
         }.call(exports, __webpack_require__(1)))

         /***/
      }),
/* 48 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         if (typeof Object.create === 'function') {
            // implementation from standard node.js 'util' module
            module.exports = function inherits(ctor, superCtor) {
               ctor.super_ = superCtor;
               ctor.prototype = Object.create(superCtor.prototype, {
                  constructor: {
                     value: ctor,
                     enumerable: false,
                     writable: true,
                     configurable: true
                  }
               });
            };
         } else {
            // old school shim for old browsers
            module.exports = function inherits(ctor, superCtor) {
               ctor.super_ = superCtor;
               var TempCtor = function TempCtor() { };
               TempCtor.prototype = superCtor.prototype;
               ctor.prototype = new TempCtor();
               ctor.prototype.constructor = ctor;
            };
         }

         /***/
      }),
/* 49 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

         module.exports = function isBuffer(arg) {
            return arg && (typeof arg === 'undefined' ? 'undefined' : _typeof(arg)) === 'object' && typeof arg.copy === 'function' && typeof arg.fill === 'function' && typeof arg.readUInt8 === 'function';
         };

         /***/
      }),
/* 50 */
/***/ (function (module, exports, __webpack_require__) {

         "use strict";


         module.exports = function (module) {
            if (!module.webpackPolyfill) {
               module.deprecate = function () { };
               module.paths = [];
               // module.parent = undefined by default
               if (!module.children) module.children = [];
               Object.defineProperty(module, "loaded", {
                  enumerable: true,
                  get: function get() {
                     return module.l;
                  }
               });
               Object.defineProperty(module, "id", {
                  enumerable: true,
                  get: function get() {
                     return module.i;
                  }
               });
               module.webpackPolyfill = 1;
            }
            return module;
         };

         /***/
      }),
/* 51 */
/***/ (function (module, exports) {

         /* (ignored) */

         /***/
      }),
/* 52 */
/***/ (function (module, exports, __webpack_require__) {

         __webpack_require__(15);
         module.exports = __webpack_require__(27);


         /***/
      })
/******/]);

if (typeof exports === "object" && typeof module === "object")
   module.exports = window.signalR;